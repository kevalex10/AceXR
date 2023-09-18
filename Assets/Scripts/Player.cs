using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private GameObject _XRRig;

    [SerializeField]
    private GameObject _cube;

    [SerializeField]
    private GameObject _cylinder;

    [SerializeField]
    private Renderer _cubeRenderer;

    [SerializeField]
    private InputActionAsset _actionAsset;

    [SerializeField]
    private PhotonView _photonView;

    private InputAction _changeCubeColorInputAction;

    private const string ACTION_MAP_NAME = "Action Map";
    private const string CHANGE_CUBE_COLOR_INPUT_ACTION_NAME = "ChangeCubeColor";
    private readonly Vector3 DEFAULT_PLAYER_POSITION = new Vector3(0, 0.125f, 2.93f);

    // Variable to keep track of the current color index from the colors list.
    private int _currentCubeColor = 0;

    // List to hold possible cube colors.
    private List<Color> _cubeColors = new List<Color> { Color.white, Color.red, Color.blue, Color.green };

    #region Unity Methods
    private void Awake()
    {
        // Finding the appropriate action map and input action in the action asset.
        InputActionMap actionMap = _actionAsset.FindActionMap(ACTION_MAP_NAME);
        _changeCubeColorInputAction = actionMap.FindAction(CHANGE_CUBE_COLOR_INPUT_ACTION_NAME);
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        _changeCubeColorInputAction.Enable();
        _changeCubeColorInputAction.performed += _ => ChangeCubeColor();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        _changeCubeColorInputAction.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    #region Methods
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _XRRig.transform.position = DEFAULT_PLAYER_POSITION;

        bool showCube = scene.buildIndex == 0;

        _cube.SetActive(showCube);
        _cylinder.SetActive(!showCube);
    }

    private void ChangeCubeColor()
    {
        // Ensuring only the owner of the PhotonView can change the color.
        if (_photonView.IsMine)
        {
            // Don't do anything if we don't have visible cube.
            if (!_cubeRenderer.gameObject.activeSelf)
            {
                return;
            }

            // Cycling through the color list.
            _currentCubeColor++;

            // Resetting the color index if it exceeds the maximum index in the list.
            if (_currentCubeColor > _cubeColors.Count - 1)
            {
                _currentCubeColor = 0;
            }

            // Getting the new color from the list.
            Color newColor = _cubeColors[_currentCubeColor];

            // Setting the new color locally.
            _cubeRenderer.material.color = newColor;

            // Sending an RPC to sync the color change across the network.
            _photonView.RPC(nameof(SyncColorAcrossNetwork), RpcTarget.OthersBuffered, new object[] { _currentCubeColor });
        }
    }
    #endregion

    #region Network Methods
    [PunRPC]
    // Method to sync the cube color across the network.
    private void SyncColorAcrossNetwork(int colorIndex)
    {
        // Setting the new color based on the data received from the RPC.
        _cubeRenderer.material.color = _cubeColors[colorIndex];
    }
    #endregion
}
