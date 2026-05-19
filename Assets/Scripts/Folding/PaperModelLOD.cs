using UnityEngine;

namespace PaperWings.Folding
{
    /// <summary>
    /// Basic LOD for the 3D paper models.
    /// Disables the wing tip sub-meshes when the camera is beyond a threshold distance.
    /// This reduces draw calls on lower-end devices or when the model is viewed from farther away
    /// (e.g. during orbit or selection views).
    /// 
    /// Attached automatically at runtime when a real paperModelPrefab is loaded.
    /// The hierarchy names (LeftWingTip / RightWingTip) come from the model generator.
    /// </summary>
    public class PaperModelLOD : MonoBehaviour
    {
        public Camera targetCamera;
        public float detailDistance = 4.5f;

        private Transform leftTip;
        private Transform rightTip;

        private void Start()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;

            // Find the tip bones (they have the MeshRenderers)
            leftTip = transform.Find("LeftWingTip");
            rightTip = transform.Find("RightWingTip");
        }

        private void LateUpdate()
        {
            if (targetCamera == null) return;

            float dist = Vector3.Distance(targetCamera.transform.position, transform.position);
            bool showDetail = dist < detailDistance;

            if (leftTip != null) leftTip.gameObject.SetActive(showDetail);
            if (rightTip != null) rightTip.gameObject.SetActive(showDetail);
        }
    }
}