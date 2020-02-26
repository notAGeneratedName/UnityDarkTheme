namespace MightyAttributes
{
    [Label("SceneName", FieldPosition.AfterHorizontal)]
    public class LabelledSceneIndexAttribute : SceneIndexAttribute
    {
        [CallbackName] public string SceneName { get; }

        public LabelledSceneIndexAttribute(string sceneName) => SceneName = sceneName;
    }
}