using UnityEngine.SceneManagement;

namespace MightyAttributes
{
    [Slider("MinBuildIndex", "MaxBuildIndex")]
    public class SceneIndexAttribute : MightyWrapperAttribute
    {
        private int MinBuildIndex => 0;
        private int MaxBuildIndex => SceneManager.sceneCountInBuildSettings - 1;
    }
}