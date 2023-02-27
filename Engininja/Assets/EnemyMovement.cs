using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    public PatrolLocation patrolLocation;
    public bool atTargetLocation = false;
    private Collider2D coll;
    private Rigidbody2D rb;
    private PlayerDetection pd;
    public EnemyAI enemyAI;
    public float enemySpeed;
    private float currentXVelocity = 0f;
    public float timeToMaxVelocity = 0.1f;
    public float timeUntilContinueAfterSeen = 3f;
    public GameObject roomsContainer;
    private List<Room> rooms;
    public LayerMask interactableMask;
    private const float HEURISTICS_WEIGHT = .5f;
    private bool atLocationFlipFlag = false;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        enemyAI = GetComponent<EnemyAI>();
        pd = GetComponentInChildren<PlayerDetection>();
        rooms = roomsContainer.GetComponentsInChildren<Room>().ToList();
    }

    void TraverseOwnRoom(float speed, Vector2 target)
    {
        bool isFlipped = transform.position.x > target.x;
        //Debug.Log("setting flip due to movement to " + isFlipped);
        enemyAI.FlipX = isFlipped;
        float directionMultiplier = isFlipped ? 1 : -1;
        float smoothXVel = Mathf.SmoothDamp(rb.velocity.x, speed * directionMultiplier * -1, ref currentXVelocity, timeToMaxVelocity);
        rb.velocity = new Vector2(smoothXVel, rb.velocity.y);
    }

    class RoomNode
    {
        public Room room;
        private float f = 0;
        private float h = 0;
        private float g = 0;
        public RoomNode parent;
        public Vector2 fromTransitionLocation;

        public void SetG(float value)
        {
            g = value;
            f = h + g;
        }

        public float GetG() { return g; }

        public void SetH(float value)
        {
            h = value;
            f = h + g;
        }

        public float GetF() { return f; }
    }

    RoomNode FindMinFRoomNodeInList(List<RoomNode> list)
    {
        if (list.Count == 0) return null;
        RoomNode minRoom = list[0];
        float minF = list[0].GetF();
        foreach (RoomNode room in list)
        {
            float roomF = room.GetF();
            if (roomF < minF)
            {
                minRoom = room;
                minF = roomF;
            }
        }
        return minRoom;
    }

    RoomNode CalcPathUsingAStar(Room currentRoom, Room targetRoom)
    {
        static float CalcHeuristic(Room c, Room t)
        {
            Vector2 posDiff = t.coll.bounds.center - c.coll.bounds.center;
            return (Mathf.Abs(posDiff.x) + Mathf.Abs(posDiff.y)) * HEURISTICS_WEIGHT;
        }

        List<RoomNode> openList = new();
        List<RoomNode> closedList = new();
        RoomNode startingNode = new()
        {
            room = currentRoom,
            fromTransitionLocation = currentRoom.transform.position
        };
        openList.Add(startingNode);

        while (openList.Count > 0)
        {

            RoomNode q = FindMinFRoomNodeInList(openList);
            openList.Remove(q);
            RoomTransition[] qTransitions = q.room.roomTransitions;
            for (int i = 0; i < qTransitions.Length; i++)
            {
                RoomTransition roomTransition = qTransitions[i];
                RoomNode successor = new()
                {
                    room = roomTransition.toRoom,
                    parent = q,
                    fromTransitionLocation = roomTransition.interactableGameObject.GetComponent<Collider2D>().bounds.center,
                };
                if (successor.room == targetRoom)
                {
                    return successor;
                }
                // Cost to get to parent + cost to get to this node
                if (q.parent == null)
                {
                    successor.SetG(q.GetG() + roomTransition.cost);
                }
                else
                {
                    float calcCost = Room.CalcCost(roomTransition.interactableGameObject.transform.position, q.fromTransitionLocation);
                    successor.SetG(q.GetG() + calcCost);
                }
                successor.SetH(CalcHeuristic(currentRoom, successor.room));
                // Check if a node with the same pos has a lower f, if so ignore this one
                int sameOpenlistNodeIndex = openList.FindIndex(c => c.room.coll.bounds.center == successor.room.coll.bounds.center);
                if (sameOpenlistNodeIndex != -1)
                {
                    // location has been found before
                    if (openList[sameOpenlistNodeIndex].GetF() < successor.GetF())
                    {
                        // Open list is better, ignore this one
                        continue;
                    }
                }
                int sameClosedListNodeIndex = closedList.FindIndex(c => c.room.coll.bounds.center == successor.room.coll.bounds.center);
                if (sameClosedListNodeIndex != -1)
                {
                    // location has been found before
                    if (closedList[sameClosedListNodeIndex].GetF() < successor.GetF())
                    {
                        // Open list is better, ignore this one
                        continue;
                    }
                }
                openList.Add(successor);
            }
            closedList.Add(q);
        }
        return null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.DrawLine(new Vector2(this.patrolLocation.location.x - .2f, this.patrolLocation.location.y - .2f), new Vector2(this.patrolLocation.location.x + .2f, this.patrolLocation.location.y + .2f), Color
            .green);
        Debug.DrawLine(new Vector2(this.patrolLocation.location.x - .2f, this.patrolLocation.location.y + .2f), new Vector2(this.patrolLocation.location.x + .2f, this.patrolLocation.location.y - .2f), Color
            .green);
        float speed;
        if (pd.alertState == PlayerDetection.AlertState.Idle)
        {
            speed = enemySpeed;
        }
        else
        {
            speed = enemySpeed * 2;
        }

        Vector2 targetLocation = new(this.patrolLocation.location.x, this.patrolLocation.location.y + 0.5f);
        //Debug.Log(tmpLocation);
        if (coll.bounds.Contains(targetLocation))
        {
            if (atLocationFlipFlag == false && this.patrolLocation.flipAtLocation)
            {
                atLocationFlipFlag = true;
                enemyAI.FlipX = !enemyAI.FlipX;

            }
            atTargetLocation = true;
            return;
        }
        atLocationFlipFlag = false;

        // Wait a second to move after last seeing the player
        if (pd.timeSinceLastSeenPlayer - Time.time > timeUntilContinueAfterSeen * -1)
        {
            return;
        }

        // Get the current enemy room
        Vector2 currentPos = new(transform.position.x, transform.position.y);
        Room currentRoom = rooms.Find((room) => room.coll.bounds.Contains(currentPos));
        // Get the c the target location is in
        //Debug.Log(transform.position + " : " + tmpLocation);
        Room targetRoom = rooms.Find((room) => room.coll.bounds.Contains(targetLocation));

        if (currentRoom == targetRoom || targetRoom == null)
        {
            TraverseOwnRoom(speed, targetLocation);
            return;
        }

        if (currentRoom == null)
        {
            return;
        }

        RoomNode path = CalcPathUsingAStar(currentRoom, targetRoom);
        List<RoomNode> nodes = new();
        while (path != null)
        {
            nodes.Add(path);
            if (path.parent == null) break;
            path = path.parent;
        }
        nodes.Reverse();
        //Remove the first node as there is no transition to go to
        nodes.RemoveAt(0);
        TraverseOwnRoom(speed, nodes[0].fromTransitionLocation);
        RaycastHit2D hit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, 1f, interactableMask);
        if (hit.collider && hit.collider.bounds.Contains(nodes[0].fromTransitionLocation))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable.IsInteractable)
            {
                interactable.Interact(gameObject);
            }
        }
        atTargetLocation = false;
    }
}
