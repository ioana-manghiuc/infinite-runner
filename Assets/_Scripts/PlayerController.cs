using Assets._Scripts.SaveLoad;
using System.Collections;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


namespace TempleRun.Player
{
    [RequireComponent(typeof(CharacterController),typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float _initialPlayerSpeed = 4f;
        [SerializeField]
        private float _maxiumPlayerSpeed = 30f;
        [SerializeField]
        private float _playerSpeedIncreaseRate = .1f;
        [SerializeField]
        private float _jumpHeight = 1f;
        [SerializeField]
        private float _initialGravityValue = -9.8f;
        [SerializeField]
        private LayerMask _groundLayer;
        [SerializeField]
        private LayerMask _turnLayer;
        [SerializeField]
        private LayerMask _obstacleLayer;
        [SerializeField]
        private AnimationClip _slidingAnimationClip;

        [SerializeField]
        private AnimationClip _hittingObjectClip1;
        [SerializeField]
        private AnimationClip _hittingObjectClip2;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private float scoreMultiplier = 10f;

        [SerializeField]
        private float _playerSpeed;
        private float _gravity;
        private Vector3 _movementDirection = Vector3.forward;
        private Vector3 _playerVelocity;

        private PlayerInput _playerInput;
        private InputAction _turnAction;
        private InputAction _jumpAction;
        private InputAction _slideAction;

        private CharacterController _controller;

        private ScoreSaver _scoreSaver;

        private int _slidingAnimationId;
        private int _hittingObjectAnimation1;
        private int _hittingObjectAnimation2;

        private bool _sliding = false;
        private float _score = 0;
        private bool _gameOver = false;
        private bool _isPaused = false;

        [SerializeField]
        private UnityEvent<Vector3> _turnEvent;
        [SerializeField]
        private UnityEvent<int> _scoreUpdateEvent;
        [SerializeField]
        private Camera _camera;
        private AudioSource _AS;
        private AudioSource _CameraAS;
        [SerializeField]
        private AudioClip _SFX;

        [SerializeField]
        private AudioClip _hitSFX;

        [SerializeField]
        private Canvas _AudioCanvas;
        [SerializeField] AudioMixer _mixer;
        [SerializeField]
        private Slider _soundtrackSlider;
        [SerializeField]
        private Slider _sfxSlider;

        private void Awake()
        {
            LoadVolume();
            _playerInput = GetComponent<PlayerInput>();
            _controller = GetComponent<CharacterController>();
            _turnAction = _playerInput.actions["Turn"];
            _jumpAction = _playerInput.actions["Jump"];
            _slideAction = _playerInput.actions["Slide"];
            _slidingAnimationId = Animator.StringToHash("Sliding");
            _hittingObjectAnimation1 = Animator.StringToHash("HittingObject1");
            _hittingObjectAnimation2 = Animator.StringToHash("HittingObject2");
            _AS = GetComponent<AudioSource>();
            _CameraAS = _camera.GetComponent<AudioSource>();
        }
        private void Start()
        {
            LoadVolume();
            _playerSpeed = _initialPlayerSpeed;
            _gravity = _initialGravityValue;
            Random.InitState(System.DateTime.Now.Millisecond);
            StartCoroutine(FadeInSound(_CameraAS, PlayerPrefs.GetFloat(VolumeSettings.SOUNDTRACK_KEY, 1f), 0.6f));
            _AudioCanvas.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _turnAction.performed += PlayerTurn;
            _jumpAction.performed += PlayerJump;
            _slideAction.performed += PlayerSlide;
        }
        private void OnDisable()
        {
            _turnAction.performed -= PlayerTurn;
            _jumpAction.performed -= PlayerJump;
            _slideAction.performed -= PlayerSlide;
        }
        private void Update()
        {
            if (!IsInPlayableArea(20)) 
            {
                GameOver();
                return;
            }
            if (!_isPaused && !_gameOver) 
            {
                _score += scoreMultiplier * Time.deltaTime;
                _scoreUpdateEvent.Invoke((int)_score);
                _playerSpeed += _playerSpeedIncreaseRate * Time.deltaTime;
                _controller.Move(transform.forward * _playerSpeed * Time.deltaTime);
                if (IsGrounded() && _playerVelocity.y < 0)
                {
                    _playerVelocity.y = 0f;
                }
                _playerVelocity.y += _gravity * Time.deltaTime;
                _controller.Move(_playerVelocity * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.P) && !_isPaused)
            {
                _AudioCanvas.gameObject.SetActive(true);
                _isPaused = true;
            }
            else if (Input.GetKey(KeyCode.L) && _isPaused)
            {
                if(_soundtrackSlider.value == 0 || _sfxSlider.value == 0)
                {
                    PlayerPrefs.SetFloat(VolumeSettings.MIXER_SOUNDTRACK, -80);
                    PlayerPrefs.SetFloat(VolumeSettings.MIXER_SFX, -80);
                }
                else
                {
                    PlayerPrefs.SetFloat(VolumeSettings.MIXER_SOUNDTRACK, Mathf.Log10(_soundtrackSlider.value) * 20);
                    PlayerPrefs.SetFloat(VolumeSettings.MIXER_SFX, Mathf.Log10(_sfxSlider.value) * 20);
                }
                _AudioCanvas.gameObject.SetActive(false);
                _isPaused = false;
            }

            LoadVolume();
        }

        private bool IsInPlayableArea(float length = 0.2f)
        {
            bool inTurnArea = false;

            Vector3 raycastOriginFirst = transform.position;
            raycastOriginFirst.y -= _controller.height / 2f;
            raycastOriginFirst.y += 0.1f;

            Vector3 raycastOriginSecond = raycastOriginFirst;
            raycastOriginFirst -= transform.forward * .2f;
            raycastOriginSecond += transform.forward * .2f;

            if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, _turnLayer)
                || Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, _turnLayer))
                inTurnArea = true;

            return inTurnArea || IsGrounded(length);
        }
        private void PlayerTurn(InputAction.CallbackContext context)
        {
            Vector3? turnPosition = CheckTurn(context.ReadValue<float>());
           
            if (!turnPosition.HasValue)
            {
                if (IsGrounded())
                    return;

                GameOver();
                return;
            }

            var targetDirection = Quaternion.AngleAxis(90 * context.ReadValue<float>(), Vector3.up)
                * _movementDirection;
            _turnEvent.Invoke(targetDirection);
            Turn(context.ReadValue<float>(), turnPosition.Value);
        }

        private void Turn(float turnValue, Vector3 turnPosition)
        {
            var tempPlayerPosition = new Vector3(turnPosition.x, transform.position.y, turnPosition.z);
            _controller.enabled = false;
            transform.position = tempPlayerPosition;
            _controller.enabled = true;

            while (_controller.enabled == false)
                _controller.enabled = true;

            Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90 * turnValue, 0);
            transform.rotation = targetRotation;
            _movementDirection = transform.forward.normalized;

        }

        private Vector3? CheckTurn(float turnValue)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, .1f, _turnLayer);
            if (hitColliders.Length != 0)
            {
                Tile tile = hitColliders[0].transform.parent.GetComponent<Tile>();
                TileType type = tile.type;

                if ((type == TileType.LEFT && turnValue == -1)
                    || (type == TileType.RIGHT && turnValue == 1)
                    || (type == TileType.SIDEWAYS))
                    return tile.pivot.position;
            }

            return null;
        }

        private void PlayerSlide(InputAction.CallbackContext context)
        {
            if (!_sliding && IsGrounded())
            {
                StartCoroutine(Slide());
                _AS.clip = _SFX;
                _AS.volume = PlayerPrefs.GetFloat(VolumeSettings.SFX_KEY, 1f);
                _AS.Play();
            }
        }

        private IEnumerator Slide()
        {
            _sliding = true;

            //Shrink the collider
            Vector3 originalControllerCenter = _controller.center;
            var newControllerCenter = originalControllerCenter;
            _controller.height /= 2;
            newControllerCenter.y -= _controller.height / 2;
            _controller.center = newControllerCenter;


            //Play the sliding animation
            _animator.Play(_slidingAnimationId);
            yield return new WaitForSeconds(_slidingAnimationClip.length);

            //retrun the collider to its original size
            _controller.height *= 2;
            _controller.center = originalControllerCenter;

            _sliding = false;

        }
        private void PlayerJump(InputAction.CallbackContext context)
        {
            if (!IsGrounded()) return;
            _playerVelocity.y += Mathf.Sqrt(_jumpHeight * _gravity * -3f);
            _controller.Move(_playerVelocity * Time.deltaTime);
            _AS.clip = _SFX;
            _AS.volume = PlayerPrefs.GetFloat(VolumeSettings.SFX_KEY, 1f); 
            _AS.Play();
        }

        private bool IsGrounded(float length = .2f)
        {
            Vector3 raycastOriginFirst = transform.position;
            raycastOriginFirst.y -= _controller.height / 2f;
            raycastOriginFirst.y += 0.1f;

            Vector3 raycastOriginSecond = raycastOriginFirst;
            raycastOriginFirst -= transform.forward * .2f;
            raycastOriginSecond += transform.forward * .2f;

            if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, _groundLayer)
               || Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, _groundLayer))
                return true;

            return false;
        }
        private IEnumerator FadeInSound(AudioSource audioSource, float fadeDuration, float targetVolume = 1.0f)
        {
            audioSource.volume = 0f; 
            audioSource.Play();      

            while (audioSource.volume < targetVolume)
            {
                audioSource.volume += targetVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }
        }

        private IEnumerator FadeOutSound(AudioSource audioSource, float fadeDuration)
        {
            float startVolume = audioSource.volume;

            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }

            audioSource.Stop(); 
            audioSource.volume = startVolume;
        }

        private void GameOver()
        {
            if (_gameOver == true) return;
            _gameOver = true;
            _scoreSaver = FindAnyObjectByType<ScoreSaver>();
            _scoreSaver.Save();
            StartCoroutine(HandleGameOver());
        }

        private IEnumerator HandleGameOver()
        {
            StartCoroutine(FadeOutSound(_CameraAS, 0.7f)); 
            _AS.clip = _hitSFX;
            _AS.Play();
            yield return StartCoroutine(Falling());
            SceneManager.LoadScene("GameOver");     
        }
        private IEnumerator Falling()
        {
            _playerSpeed = 0;
            
            float animationDuration;
            int randomChoice = Random.Range(0, 2);
            if (randomChoice % 2 != 0)
            {
                _animator.Play(_hittingObjectAnimation1);
                animationDuration = _hittingObjectClip1.length;
            }
            else
            {
                _animator.Play(_hittingObjectAnimation2);
                animationDuration = _hittingObjectClip2.length;
            }

            yield return new WaitForSeconds(animationDuration * 0.8f);

        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if ((( 1 << hit.collider.gameObject.layer) & _obstacleLayer) != 0) 
            {
                GameOver();
            }
        }
        void LoadVolume()
        {
            float soundtrackVolume = PlayerPrefs.GetFloat(VolumeSettings.SOUNDTRACK_KEY, 1f);
            float sfxVolume = PlayerPrefs.GetFloat(VolumeSettings.SFX_KEY, 1f);
            if(soundtrackVolume != 0)
                _mixer.SetFloat(VolumeSettings.MIXER_SOUNDTRACK, Mathf.Log10(soundtrackVolume) * 20);
            else
                _mixer.SetFloat(VolumeSettings.MIXER_SOUNDTRACK, -80);

            if(sfxVolume != 0)
                _mixer.SetFloat(VolumeSettings.MIXER_SFX, Mathf.Log10(sfxVolume) * 20);
            else
                _mixer.SetFloat(VolumeSettings.MIXER_SFX, -80);
        }
    }
}