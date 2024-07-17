using System;
using System.Collections;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine;
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

        // [SerializeField] private float v1, v2 = 30f, v3 = 90f;
        // [SerializeField] private int vibrato = 10;
        //
        // [SerializeField] private bool fadeOut;
        // [SerializeField] private Ease Ease;

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

            if (_contentRoot.Geometry is not GeomTransforms gt)
            {
                throw new InvalidCastException();
            }


            gt.SetScale(0.5F)
                .MoveTo(ViewHorizontalAlign.Center, ViewVerticalAlign.Top)
                .Offset(new Vector2(0, -GameInsView.ScreenSize.y * (1 - value)));

            // .DoFade(new Color(1, 1, 1, 0.5F), 5);

            // yield return _contentRoot.DisplayFaceFade("ai_a_0028");

            yield return new WaitForSeconds(1);

            // yield return _contentRoot.DisplayBodyFade("ai_a_0002");

            // yield return gt.WaitForDoNod();
            //
            // yield return gt.WaitForDoShake();
            //
            // yield return gt.WaitForDoShakeHead();

            yield return gt.WaitForDoJump();
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    yield return gt.WaitForDoJump();
                }

                yield return null;
            }
        }
    }
}