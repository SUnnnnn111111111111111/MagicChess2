using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class CameraSwitchController : MonoBehaviour
{
    public Transform whiteTeamView;
    public Transform blackTeamView;
    [SerializeField] private AnimationCurve curve;
    public float moveDuration = 3f;

    private void Start()
    {
        GameStateManager.Instance.OnGameStateChanged.AddListener(HandleStateChange);
    }

    private void HandleStateChange(GameStateManager.GameState newState)
    {
        switch (newState)
        {
            case GameStateManager.GameState.WhitePlaying:
                MoveCamera(whiteTeamView.position, whiteTeamView.rotation);
                break;
            case GameStateManager.GameState.BlackPlaying:
                MoveCamera(blackTeamView.position, blackTeamView.rotation);
                break;
        }
    }

    private void MoveCamera(Vector3 position, Quaternion rotation)
    {
        transform.DOMove(position, moveDuration).SetEase(curve);
        transform.DORotateQuaternion(rotation, moveDuration).SetEase(curve);
    }
}