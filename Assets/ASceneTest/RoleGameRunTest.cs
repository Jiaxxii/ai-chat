using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Xiyu.ExpandMethod;
using Xiyu.GameFunction.CharacterComponent;
using Xiyu.GameFunction.GeometricTransformations;
using Xiyu.GameFunction.SceneView;

namespace ASceneTest
{
    public class RoleGameRunTest : MonoBehaviour
    {
        [SerializeField] [Range(0F, 1F)] private float value;
        [SerializeField] private TextAsset textAsset;

        [SerializeField] private CharacterContentRoot characterContentRoot;
        private ICharacterControl _contentRoot;

        private IEnumerator Start()
        {
            if (characterContentRoot is not null)
            {
                _contentRoot = characterContentRoot;
                yield return _contentRoot.Init("ai", JObject.Parse(textAsset.text), true);
            }
            else
            {
                yield return CharacterContentRoot.CreateRole<CharacterContentRoot>(
                    transform,
                    "ai",
                    JObject.Parse(textAsset.text),
                    control => _contentRoot = control,
                    true);
            }


            yield return _contentRoot.Display("ai_a_0006", "ai_a_0026");

            _contentRoot.Geometry.SetScale(0.5F);
            // _contentRoot.Geometry.Angle(90F);

            _contentRoot.Geometry.MoveTo(ViewHorizontalAlign.Center, ViewVerticalAlign.Top);

            _contentRoot.Geometry.Offset(new Vector2(0, -GameInsView.ScreenSize.y * (1 - value)));

            yield break;
        }
    }
}