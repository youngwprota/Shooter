using System.Linq;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class Movement : MovementBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Audio Clips")]
        
        [Tooltip("The audio clip that is played while walking.")]
        [SerializeField]
        private AudioClip audioClipWalking;

        [Tooltip("The audio clip that is played while running.")]
        [SerializeField]
        private AudioClip audioClipRunning;

        [Tooltip("The audio clip that играет при прыжке.")]
        [SerializeField]
        private AudioClip audioClipJump;

        [Header("Speeds")]

        [SerializeField]
        private float speedWalking = 5.0f;

        [Tooltip("How fast the player moves while running."), SerializeField]
        private float speedRunning = 9.0f;

        [Header("Jump Settings")]

        [Tooltip("Jump force applied when the player jumps.")]
        [SerializeField]
        private float jumpForce = 7f;

        [Tooltip("Cooldown between jumps.")]
        [SerializeField]
        private float jumpCooldown = 0.5f;

        #endregion

        #region PROPERTIES

        //Velocity.
        private Vector3 Velocity
        {
            //Getter.
            get => rigidBody.velocity;
            //Setter.
            set => rigidBody.velocity = value;
        }

        #endregion

        #region FIELDS

        private Rigidbody rigidBody;
        private CapsuleCollider capsule;
        private AudioSource audioSource;

        private bool grounded;
        private bool canJump = true;

        private CharacterBehaviour playerCharacter;
        private WeaponBehaviour equippedWeapon;

        private readonly RaycastHit[] groundHits = new RaycastHit[8];

        #endregion

        #region UNITY FUNCTIONS

        protected override void Awake()
        {
            playerCharacter = ServiceLocator.Current.Get<IGameModeService>().GetPlayerCharacter();
        }

        protected override void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

            capsule = GetComponent<CapsuleCollider>();

            audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClipWalking;
            audioSource.loop = true;
        }

        private void OnCollisionStay()
        {
            Bounds bounds = capsule.bounds;
            Vector3 extents = bounds.extents;
            float radius = extents.x - 0.01f;

            Physics.SphereCastNonAlloc(bounds.center, radius, Vector3.down,
                groundHits, extents.y - radius * 0.5f, ~0, QueryTriggerInteraction.Ignore);

            grounded = groundHits.Any(hit => hit.collider != null && hit.collider != capsule);
        }

        protected override void FixedUpdate()
        {
            MoveCharacter();
            grounded = false;
        }

        protected override void Update()
        {
            equippedWeapon = playerCharacter.GetInventory().GetEquipped();
            PlayFootstepSounds();
            HandleJump();
        }

        #endregion

        #region METHODS

        private void MoveCharacter()
        {
            Vector2 frameInput = playerCharacter.GetInputMovement();
            var movement = new Vector3(frameInput.x, 0.0f, frameInput.y);

            movement *= playerCharacter.IsRunning() ? speedRunning : speedWalking;
            movement = transform.TransformDirection(movement);
            Velocity = new Vector3(movement.x, rigidBody.velocity.y, movement.z);
        }

        private void PlayFootstepSounds()
        {
            if (grounded && rigidBody.velocity.sqrMagnitude > 0.1f)
            {
                audioSource.clip = playerCharacter.IsRunning() ? audioClipRunning : audioClipWalking;
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
            else if (audioSource.isPlaying)
                audioSource.Pause();
        }

        /// <summary>
        /// Обработка прыжка.
        /// </summary>
        private void HandleJump()
        {
            // Проверяем нажатие пробела, наличие земли и перезарядку прыжка
            if (Input.GetKeyDown(KeyCode.Space) && grounded && canJump)
            {
                rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                grounded = false;
                canJump = false;

                // Воспроизводим звук прыжка, если есть
                if (audioClipJump != null)
                {
                    audioSource.PlayOneShot(audioClipJump);
                }

                // Запускаем кулдаун
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }

        /// <summary>
        /// Сбрасываем возможность прыжка после кулдауна.
        /// </summary>
        private void ResetJump()
        {
            canJump = true;
        }

        #endregion
    }
}
