using FlaxEngine;

namespace Game
{
    public class Character : Script
    {
        // Groups names for UI
        private const string MovementGroup = "Movement";
        private const string CameraGroup = "Camera";

        public Actor CharacterObj { get; set; } = null;

        [Range(0f, 300f)]
        public float Speed { get; set; } = 250;

        [Range(0f, 300f)]
        public float SprintSpeed { get; set; } = 300;

        [Limit(-20f, 20f)]
        public float Gravity { get; set; } = -9.81f;

        [Range(0f, 25f)]
        public float JumpStrength { get; set; } = 10;
        
        public Camera CameraView { get; set; } = null;

        [Range(0, 10f)]
        public float MouseSensitivity { get; set; } = 100f;

        [Range(0f, 20f)]
        public float CameraLag { get; set; } = 10;

        [Range(0f, 100f)]
        public float FOVZoom { get; set; } = 50;
     
        public Vector2 PitchMinMax { get; set; } = new Vector2(-45, 45);

        private CharacterController _controller;
        private Vector3 _velocity;
        public Actor PlayerModel;
        public AnimGraphParameter parametterx;
        public AnimGraphParameter paramettery;
        private float _yaw;
        private float _pitch;
        private float _defaultFov;
        
 
        public override void OnStart()
        {
            _controller = (CharacterController)Parent;
            if (!CameraView || !CharacterObj)
            {
                Debug.LogError("No Character or Camera assigned!");
                return;
            }

            _defaultFov = CameraView.FieldOfView;
            PlayerModel.FindActor("PlayerModel");
            parametterx = PlayerModel.As<AnimatedModel>().GetParameter("x");
            paramettery = PlayerModel.As<AnimatedModel>().GetParameter("y");
        }

        public override void OnUpdate()
        {
            Screen.CursorLock = CursorLockMode.Locked;
            Screen.CursorVisible = false;    
            
        }

        public override void OnFixedUpdate()
        {
            {
                _yaw += Input.GetAxis("Mouse X") * MouseSensitivity * Time.DeltaTime; // H
                _pitch += Input.GetAxis("Mouse Y") * MouseSensitivity * Time.DeltaTime; // V
                _pitch = Mathf.Clamp(_pitch, PitchMinMax.X, PitchMinMax.Y);


                CameraView.Parent.Orientation = Quaternion.Lerp(CameraView.Parent.Orientation, Quaternion.Euler(_pitch, _yaw, 0), Time.DeltaTime * CameraLag);
                CharacterObj.Orientation = Quaternion.Euler(0, _yaw, 0);

                if (Input.GetAction("Aim"))
                {
                    CameraView.FieldOfView = Mathf.Lerp(CameraView.FieldOfView, FOVZoom, Time.DeltaTime * 5f);
                }
                else
                {
                    CameraView.FieldOfView = Mathf.Lerp(CameraView.FieldOfView, _defaultFov, Time.DeltaTime * 5f);
                }
            }

            {
                
                var inputH = Input.GetAxis("Horizontal");
                var inputV = Input.GetAxis("Vertical");
                parametterx.Value = inputH;
                paramettery.Value = inputV;


                var movement = new Vector3(inputH, 0.0f, inputV);
                var movementDirection = CameraView.Transform.TransformDirection(movement);

                if (_controller.IsGrounded && Input.GetAction("Jump"))
                {
                    _velocity.Y = Mathf.Sqrt(JumpStrength * -2f * Gravity);                   
                }

                _velocity.Y += Gravity * Time.DeltaTime;
                movementDirection += (_velocity * 0.5f);

                _controller.Move(movementDirection * Time.DeltaTime * (Input.GetAction("Sprint") ? SprintSpeed : Speed));
            }
        }
    }
}
