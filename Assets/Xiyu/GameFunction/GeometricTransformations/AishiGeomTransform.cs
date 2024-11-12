using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Xiyu.VirtualLiveRoom.Component.Character.Emotion;

namespace Xiyu.GameFunction.GeometricTransformations
{
    public class AishiGeomTransform : GeomTransforms
    {
        public AishiGeomTransform(Vector2 windowSize, Property<Vector2> positionSetter, Property<Vector2> sizeSetter, Property<Vector3> scaleSetter, Property<Vector3> rotateSetter)
            : base(windowSize, positionSetter, sizeSetter, scaleSetter, rotateSetter)
        {
        }

        public AishiGeomTransform(Vector2 windowSize, Property<Vector2> positionSetter, Property<Vector2> sizeSetter, Property<Vector3> scaleSetter, Property<Vector3> rotateSetter,
            Property<Color> colorProperty) : base(windowSize, positionSetter, sizeSetter, scaleSetter, rotateSetter, colorProperty)
        {
        }


        public async UniTask EmotionToAction(CharacterBasicEmotions emotion)
        {
            var actionTween = emotion switch
            {
                CharacterBasicEmotions.Happiness => DoJump(),
                CharacterBasicEmotions.Sadness => DoShake(),
                CharacterBasicEmotions.Anger => DoJump(),
                CharacterBasicEmotions.Fear => DoShake(),
                CharacterBasicEmotions.Surprise => DoShakeHead(),
                CharacterBasicEmotions.Disgust => DoNod(),
                CharacterBasicEmotions.Guilt => DoShake(),
                CharacterBasicEmotions.Excitement => DoJump(),
                CharacterBasicEmotions.Jealousy => DoShakeHead(),
                CharacterBasicEmotions.Love => DoJump(),
                CharacterBasicEmotions.Shame => DoJump(),
                CharacterBasicEmotions.Embarrassment => DoJump(),
                CharacterBasicEmotions.Satisfaction => DoShake(),
                CharacterBasicEmotions.Frustration => DoShakeHead(),
                CharacterBasicEmotions.Anticipation => DoJump(),
                CharacterBasicEmotions.Admiration => DoNod(),
                CharacterBasicEmotions.Worship => DoJump(),
                CharacterBasicEmotions.Appreciation => DoShake(),
                CharacterBasicEmotions.Awe => DoJump(),
                CharacterBasicEmotions.Boredom => DoShakeHead(),
                CharacterBasicEmotions.Confusion => DoJump(),
                CharacterBasicEmotions.Desire => DoShake(),
                CharacterBasicEmotions.Fascination => DoNod(),
                CharacterBasicEmotions.Nostalgia => DoJump(),
                CharacterBasicEmotions.Romance => DoShake(),
                CharacterBasicEmotions.SexualDesire => DoShakeHead(),
                CharacterBasicEmotions.Sympathy => DoJump(),
                CharacterBasicEmotions.Disdain => DoJump(),
                CharacterBasicEmotions.Contempt => DoNod(),
                CharacterBasicEmotions.Anxiety => DoShakeHead(),
                CharacterBasicEmotions.Depression => DoShakeHead(),
                CharacterBasicEmotions.Gratitude => DoNod(),
                CharacterBasicEmotions.Pride => DoJump(),
                CharacterBasicEmotions.Calmness => DoNod(),
                _ => DoNod(),
            };

            await actionTween.AsyncWaitForCompletion().AsUniTask();
        }
    }
}