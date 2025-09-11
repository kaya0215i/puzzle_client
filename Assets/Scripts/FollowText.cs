using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowText : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    private RectTransform rectTransform;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update() {
        if(target == null) {
            return;
        }

        rectTransform.position = Camera.main.WorldToScreenPoint(target.position + offset);
    }
}
