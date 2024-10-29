using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Xiyu.GameFunction.GeometricTransformations;

namespace Xiyu.VirtualLiveRoom.Component.Character
{
    public interface ICharacterControl
    {
        /// <summary>
        /// 强烈建议 <see cref="Type"/> 名称与要加载的资源标签名称相同
        /// </summary>
        string Type { get; }


        /// <summary>
        /// Body 节点控制器
        /// </summary>
        RoleBodyType Body { get; }

        /// <summary>
        /// Faces 节点控制器
        /// </summary>
        RoleBodyType Faces { get; }

        /// <summary>
        /// 几何变换
        /// </summary>
        IGeomTransforms Geometry { get; }

        /// <summary>
        /// 显示立绘
        /// </summary>
        /// <param name="bodyCode">身体</param>
        /// <param name="faceCode">脸部</param>
        /// <returns>此方法需要配合协程使用</returns>
        UniTask Display(string bodyCode, string faceCode);

        UniTask DisplayFaceFade(string faceCode, float outDuration = 0.2F, float inDuration = 0.2F);
        UniTask DisplayBodyFade(string bodyCode, float outDuration = 0.25F, float inDuration = 0.33F);


        UniTask Init(Vector2 windowSize, string roleTye);


        [CanBeNull]
        T GetThis<T>() where T : UnityEngine.Component, ICharacterControl;
    }
}