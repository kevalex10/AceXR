using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Get the current active scene.
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Determine the next scene index.
            int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;

            PhotonNetwork.LoadLevel(nextSceneIndex);
        }
    }
}
