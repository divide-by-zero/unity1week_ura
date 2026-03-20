using System;
using System.Collections.Generic;
using System.Linq;
using InfinitePorker.Manager;
using KszUtil;
using UnityEngine;
using UnityEngine.UI;

namespace InfinitePorker
{
    public class DragLineRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Image _startImage;
        [SerializeField] private Image _endImage;
        [SerializeField] private Color _beginColor = Color.green;
        [SerializeField] private Color _endColor = Color.aliceBlue;
        [SerializeField] private CanvasGroup _canvasGroup;

        private void Reset()
        {
            TryGetComponent(out _lineRenderer);
        }

        private void Start()
        {
            _startImage.color = _beginColor;
            _endImage.color = _endColor;

            //LineRendererの初期設定 _beginColorと_endColorを適用
            _lineRenderer.startColor = _beginColor;
            _lineRenderer.endColor = _endColor;

            SetDragLinePos(null);
        }

        public void SetDragLinePos(IEnumerable<Vector2> positions)
        {
            if (positions == null || positions.Count() < 2)
            {
                _startImage.enabled = false;
                _endImage.enabled = false;
                _lineRenderer.enabled = false;
                return;
            }

            _startImage.enabled = true;
            _endImage.enabled = true;
            _lineRenderer.enabled = true;
            var posArray = positions.Select(v => new Vector3(v.x, v.y, 0)).ToArray();

            _startImage.transform.position = posArray.First();
            _endImage.transform.position = posArray.Last();

            if (posArray.Length >= 2)
            {
                var startDirection = (posArray[1] - posArray[0]).normalized;
                _startImage.transform.rotation = Quaternion.LookRotation(Vector3.forward, startDirection);

                var endDirection = (posArray[posArray.Length - 1] - posArray[posArray.Length - 2]).normalized;
                _endImage.transform.rotation = Quaternion.LookRotation(Vector3.forward, endDirection);
            }

            _lineRenderer.positionCount = posArray.Length;
            _lineRenderer.SetPositions(posArray);
        }

        public void SetAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
            _lineRenderer.startColor = _beginColor.SetAlpha(alpha);
            _lineRenderer.endColor = _endColor.SetAlpha(alpha);
        }
    }
}