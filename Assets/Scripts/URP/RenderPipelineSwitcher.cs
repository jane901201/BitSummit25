using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class RenderPipelineSwitcher : MonoBehaviour
{
    public RenderPipelineAsset urpVolumetricFogLite;
    public RenderPipelineAsset pcRPAsset;

    private void Awake()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SwitchPipeline(currentSceneName);
    }

    public void SwitchPipeline(string sceneName)
    {
        if (sceneName == "GameScene ParametaTyousei")
        {
            GraphicsSettings.defaultRenderPipeline = urpVolumetricFogLite;
        }
        else
        {
            GraphicsSettings.defaultRenderPipeline = pcRPAsset;
        }
    }
}
