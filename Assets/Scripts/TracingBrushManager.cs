using UnityEngine;

public class TracingBrushManager : MonoBehaviour
{
    public TracingPath[] paths;
    public GameObject brushDotPrefab;
    public Transform brushParent;
    public float brushSpacing = 0.1f;

    private int currentStrokeIndex = 0;
    private int currentCheckpointIndex = 0;
    private Vector3 lastPos;
    private bool isTracing = false;

    private TracingLevelManager levelManager;

    void Start()
    {
        levelManager = FindObjectOfType<TracingLevelManager>();

        // Disable all checkpoints at the beginning
        foreach (var path in paths)
            if (path != null) path.EnableCheckpoints(false);

        // Enable first 2 checkpoints of the first stroke
        if (paths.Length > 0 && paths[0].checkpoints.Length > 1)
        {
            paths[0].EnableCheckpoint(0, true);
            paths[0].EnableCheckpoint(1, true);
        }
    }

    void Update()
    {
        if (currentStrokeIndex >= paths.Length) return;

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;

        var currentPath = paths[currentStrokeIndex];

        // Start tracing when clicking the correct checkpoint
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D hit = Physics2D.OverlapPoint(pos);
            if (hit != null && currentPath.checkpoints.Length > 0 &&
                hit.GetComponent<Checkpoint>() == currentPath.checkpoints[currentCheckpointIndex])
            {
                isTracing = true;
                lastPos = pos;
            }
        }

        // While dragging → spawn brush dots inside draw area
        if (Input.GetMouseButton(0) && isTracing)
        {
            if (currentPath.drawArea != null && currentPath.drawArea.OverlapPoint(pos))
            {
                if (Vector3.Distance(lastPos, pos) > brushSpacing)
                {
                    Instantiate(brushDotPrefab, pos, Quaternion.identity, brushParent);
                    lastPos = pos;
                }
            }
        }

        // When released → check if reached next checkpoint
        if (Input.GetMouseButtonUp(0) && isTracing)
        {
            if (currentPath.checkpoints.Length > currentCheckpointIndex + 1)
            {
                Collider2D hit = Physics2D.OverlapPoint(pos);

                // ✅ Correct next checkpoint
                if (hit != null && hit.GetComponent<Checkpoint>() == currentPath.checkpoints[currentCheckpointIndex + 1])
                {
                    currentPath.EnableCheckpoint(currentCheckpointIndex, false);
                    currentCheckpointIndex++;

                    // Keep current + next checkpoint active
                    if (currentCheckpointIndex < currentPath.checkpoints.Length - 1)
                    {
                        currentPath.EnableCheckpoint(currentCheckpointIndex, true);
                        currentPath.EnableCheckpoint(currentCheckpointIndex + 1, true);
                    }
                    else
                    {
                        // Stroke finished
                        if (currentPath.strokeFill != null)
                            currentPath.strokeFill.SetActive(true);

                        ClearBrushDots();
                        currentPath.EnableCheckpoints(false);

                        // Notify LevelManager
                        if (levelManager != null)
                            levelManager.OnStrokeCompleted();

                        // Move to next stroke
                        currentStrokeIndex++;
                        currentCheckpointIndex = 0;

                        // Enable checkpoints for next stroke
                        if (currentStrokeIndex < paths.Length && paths[currentStrokeIndex].checkpoints.Length > 1)
                        {
                            paths[currentStrokeIndex].EnableCheckpoint(0, true);
                            paths[currentStrokeIndex].EnableCheckpoint(1, true);
                        }
                        else
                        {
                            Debug.Log("🏆 Shape/Letter Completed!");
                            if (paths.Length > 0 && paths[0].parentFill != null)
                                paths[0].parentFill.SetActive(true);
                        }
                    }
                }
                else
                {
                    // ❌ Wrong checkpoint → reset dots
                    ClearBrushDots();
                }
            }

            isTracing = false;
            lastPos = Vector3.zero;
        }
    }

    public int GetStrokeCount()
    {
        return (paths != null) ? paths.Length : 0;
    }

    // Reset everything so the shape/letter can be retraced
    public void ResetTracing()
    {
        isTracing = false;
        lastPos = Vector3.zero;
        currentStrokeIndex = 0;
        currentCheckpointIndex = 0;

        ClearBrushDots();

        if (paths != null)
        {
            foreach (var p in paths)
            {
                if (p == null) continue;
                if (p.strokeFill != null) p.strokeFill.SetActive(false);
                if (p.parentFill != null) p.parentFill.SetActive(false);
                p.EnableCheckpoints(false);
            }

            // Enable first checkpoints of the very first stroke
            if (paths.Length > 0 && paths[0] != null && paths[0].checkpoints != null)
            {
                if (paths[0].checkpoints.Length > 1)
                {
                    paths[0].EnableCheckpoint(0, true);
                    paths[0].EnableCheckpoint(1, true);
                }
                else if (paths[0].checkpoints.Length == 1)
                {
                    paths[0].EnableCheckpoint(0, true);
                }
            }
        }
    }

    private void ClearBrushDots()
    {
        if (brushParent != null)
        {
            foreach (Transform child in brushParent)
                Destroy(child.gameObject);
        }
    }
}

[System.Serializable]
public class TracingPath
{
    public string strokeName;
    public Checkpoint[] checkpoints;
    public Collider2D drawArea;
    public GameObject strokeFill;
    public GameObject parentFill;

    public void EnableCheckpoints(bool enable)
    {
        foreach (var cp in checkpoints)
            if (cp != null) cp.gameObject.SetActive(enable);
    }

    public void EnableCheckpoint(int index, bool enable)
    {
        if (index >= 0 && index < checkpoints.Length && checkpoints[index] != null)
            checkpoints[index].gameObject.SetActive(enable);
    }
}
