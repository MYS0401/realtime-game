using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class MoveOnSpline3D_Switch : MonoBehaviour
{
    public List<SplineContainer> splines; // すべてのスプライン
    public int currentSplineIndex = 0;     // 現在のスプライン
    public float moveSpeed = 0.2f;
    public float rotateSpeed = 10f;

    Rigidbody rb;

    RoomModel roomModel;

    public float t = 0;
    private CharacterController controller;

    public GameManager gameManager;

    bool justWarped = false;

    //接触クールタイム
    float lastContactTime = -1f;
    const float CONTACT_COOLDOWN = 0.2f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        roomModel = FindObjectOfType<RoomModel>();

        t = 0.11f;
    }

    void Update()
    {
        //カウントダウン中は入力を受け付けない
        if (InputBlocker.isBlocked) return;
        if (splines.Count == 0) return;

        var spline = splines[currentSplineIndex].Spline;
        if (spline.Count < 2) return;

        float input = Input.GetAxis("Horizontal");
        float delta = input * moveSpeed * Time.deltaTime;

        // ループ対応
        t = (t + delta) % 1f;
        if (t < 0) t += 1f;
      
            // 分岐判定
            if (currentSplineIndex == 0)
            {//白線上分岐地点
                if (Input.GetKeyUp(KeyCode.E))
                {
                    if (t >= 0.06f && 0.07f >= t)
                    {
                        currentSplineIndex = 1;
                        t = 0.91f;
                    justWarped = true;
                    }

                    if (t >= 0.182f && 0.188f >= t)
                    {
                        currentSplineIndex = 1;
                        t = 0.192f;
                    justWarped = true;
                    }

                    if (t >= 0.558f && 0.564f >= t)
                    {
                        currentSplineIndex = 1;
                        t = 0.412f;
                    justWarped = true;
                    }

                    if (t >= 0.682f && 0.687f >= t)
                    {
                        currentSplineIndex = 1;
                        t = 0.694f;
                    justWarped = true;
                    }
                }
            }
            else if (currentSplineIndex == 1)
            {//青線上分岐地点
                if (Input.GetKeyUp(KeyCode.E))
                {
                    if (t >= 0.9f && 0.95f >= t)
                    {
                        currentSplineIndex = 0;
                        t = 0.065f;
                    }

                    if (t >= 0.19f && 0.196f >= t)
                    {
                        currentSplineIndex = 0;
                        t = 0.184f;
                    }

                    if (t >= 0.41f && 0.415f >= t)
                    {
                        currentSplineIndex = 0;
                        t = 0.56f;
                    }

                    if (t >= 0.692f && 0.696f >= t)
                    {
                        currentSplineIndex = 0;
                        t = 0.684f;
                    }
                }
            }
        

        
        Vector3 pos = splines[currentSplineIndex].EvaluatePosition(t);
        transform.position = pos;

        Vector3 tangent = splines[currentSplineIndex].EvaluateTangent(t);
        if (tangent.sqrMagnitude > 0.0001f)
        {
            Vector3 dir = tangent.normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - lastContactTime < CONTACT_COOLDOWN)
            return;

        if (!other.CompareTag("RemotePlayer")) return;

        var remote = other.GetComponent<RemotePlayer>();
        if (remote == null) return;

        lastContactTime = Time.time;
        roomModel.NotifyContactAsync(remote.ConnectionId);
    }
}
