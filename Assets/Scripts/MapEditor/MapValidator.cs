using UnityEngine;

public class MapValidator : MonoBehaviour
{
    public enum MapState
    {
        Validated,
        Edited,
        Empty
    }

    private MapState _mapState;

    public MapState MapStateValue { get => _mapState; set => _mapState = value; }

    private void Start()
    {
        _mapState = MapState.Empty;
    }
}
