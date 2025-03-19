using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class CameraSwitchController : MonoBehaviour
{
    public Transform whiteTeamView;
    public Transform blackTeamView;
    [SerializeField] private AnimationCurve curve;
    public float delay = 0.5f;
    public float moveDuration = 3f;

    private void Start()
    {
        GameStateManager.Instance.OnGameStateChanged.AddListener(HandleStateChange);
    }

    private void HandleStateChange(GameStateManager.GameState newState)
    {
        switch (newState)
        {
            case GameStateManager.GameState.WhitesPlaying:
                MoveCamera(whiteTeamView.position, whiteTeamView.rotation);
                break;
            case GameStateManager.GameState.BlacksPlaying:
                MoveCamera(blackTeamView.position, blackTeamView.rotation);
                break;
        }
    }

    private void MoveCamera(Vector3 position, Quaternion rotation)
    {
        RTS_Cam.RTS_Camera rtsCamera = GetComponent<RTS_Cam.RTS_Camera>();
        if (rtsCamera != null)
        {
            DG.Tweening.DOVirtual.DelayedCall(delay, () =>
            {
                rtsCamera.ResetOrthographicSize();
            });
        }
    
        transform.DOMove(position, moveDuration).SetEase(curve).SetDelay(delay);
        transform.DORotateQuaternion(rotation, moveDuration).SetEase(curve).SetDelay(delay);
    }
}