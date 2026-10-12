using UnityEngine;

public class CameraManager : ManagerBase
{
    const float DeadZoneHalfWidth = 2f;
    const float DeadZoneHalfHeight = 1.25f;
    const float FollowSmoothTime = 0.3f;
    const float ConfineSkin = 0.001f;

    Camera view;
    bool missingLogged;
    bool wallChecked;
    ContactFilter2D wallFilter;
    readonly Collider2D[] wallHits = new Collider2D[16];
    Vector3 followVelocity;

    void LateUpdate()
    {
        Follow();
    }

    public void Move()
    {
        CharacterBase player = CurrentPlayer();
        if (player == null || !EnsureView())
            return;

        SnapTo(player.transform.position);
        followVelocity = Vector3.zero;
    }

    void Follow()
    {
        CharacterBase player = CurrentPlayer();
        if (player == null || !EnsureView())
            return;

        Vector3 position = view.transform.position;
        Vector3 goal = DeadZoneGoal(position, player.transform.position);
        goal = Confine(goal, position, false);
        Vector3 next = Vector3.SmoothDamp(position, goal, ref followVelocity, FollowSmoothTime);
        next.z = position.z;
        next = Confine(next, position, true);
        view.transform.position = next;
    }

    static Vector3 DeadZoneGoal(Vector3 cameraPosition, Vector3 playerPosition)
    {
        Vector3 goal = cameraPosition;
        float dx = playerPosition.x - cameraPosition.x;
        float dy = playerPosition.y - cameraPosition.y;

        if (Mathf.Abs(dx) > DeadZoneHalfWidth)
            goal.x = playerPosition.x - Mathf.Sign(dx) * DeadZoneHalfWidth;

        if (Mathf.Abs(dy) > DeadZoneHalfHeight)
            goal.y = playerPosition.y - Mathf.Sign(dy) * DeadZoneHalfHeight;

        return goal;
    }

    Vector3 Confine(Vector3 position, Vector3 reference, bool stopIntoWall)
    {
        if (!view.orthographic || !EnsureWallFilter())
            return position;

        float halfHeight = view.orthographicSize;
        float halfWidth = halfHeight * view.aspect;
        if (halfWidth <= 0f || halfHeight <= 0f)
            return position;

        Vector2 size = new Vector2(halfWidth * 2f, halfHeight * 2f);
        for (int pass = 0; pass < 4; pass++)
        {
            int count = Physics2D.OverlapBox(position, size, 0f, wallFilter, wallHits);
            if (count <= 0)
                break;

            bool pushed = false;
            int limit = count < wallHits.Length ? count : wallHits.Length;
            for (int i = 0; i < limit; i++)
            {
                Collider2D hit = wallHits[i];
                if (hit == null)
                    continue;

                if (PushOut(ref position, reference, hit.bounds, halfWidth, halfHeight, stopIntoWall))
                    pushed = true;
            }

            if (!pushed)
                break;
        }

        return position;
    }

    bool PushOut(ref Vector3 position, Vector3 reference, Bounds wall, float halfWidth, float halfHeight, bool stopIntoWall)
    {
        float overlapX = Mathf.Min(position.x + halfWidth, wall.max.x) - Mathf.Max(position.x - halfWidth, wall.min.x);
        float overlapY = Mathf.Min(position.y + halfHeight, wall.max.y) - Mathf.Max(position.y - halfHeight, wall.min.y);
        if (overlapX <= ConfineSkin || overlapY <= ConfineSkin)
            return false;

        if (overlapX < overlapY)
        {
            if (reference.x <= wall.center.x)
            {
                position.x -= overlapX;
                if (stopIntoWall)
                    followVelocity.x = Mathf.Min(followVelocity.x, 0f);
            }
            else
            {
                position.x += overlapX;
                if (stopIntoWall)
                    followVelocity.x = Mathf.Max(followVelocity.x, 0f);
            }
        }
        else if (reference.y <= wall.center.y)
        {
            position.y -= overlapY;
            if (stopIntoWall)
                followVelocity.y = Mathf.Min(followVelocity.y, 0f);
        }
        else
        {
            position.y += overlapY;
            if (stopIntoWall)
                followVelocity.y = Mathf.Max(followVelocity.y, 0f);
        }

        return true;
    }

    void SnapTo(Vector3 target)
    {
        Vector3 position = view.transform.position;
        position.x = target.x;
        position.y = target.y;
        position = Confine(position, position, false);
        view.transform.position = position;
    }

    bool EnsureView()
    {
        if (view == null)
            view = Camera.main;

        if (view != null)
            return true;

        if (!missingLogged)
        {
            missingLogged = true;
            Debug.LogError("메인 카메라 없음.");
        }

        return false;
    }

    bool EnsureWallFilter()
    {
        if (wallChecked)
            return wallFilter.layerMask != 0;

        wallChecked = true;
        int layer = LayerMask.NameToLayer("Wall");
        if (layer < 0)
        {
            Debug.LogError("월 레이어 없음.");
            return false;
        }

        wallFilter.SetLayerMask(1 << layer);
        wallFilter.useTriggers = false;
        return true;
    }

    static CharacterBase CurrentPlayer()
    {
        if (GameManager.Instance == null)
            return null;

        InGameManager inGame = GameManager.Instance.InGame;
        if (inGame == null)
            return null;

        return inGame.Player;
    }
}
