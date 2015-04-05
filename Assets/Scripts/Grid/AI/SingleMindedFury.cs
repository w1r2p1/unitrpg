﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/**
 * This AI brain on every turn just tries to seek out a target.
 * Can hold off until the target is within a certain range, as well.
 */
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Grid.Unit))]
public class SingleMindedFury : MonoBehaviour, AIStrategy {
    public GameObject Target;
    public int AggroRange = int.MaxValue;
    public Grid.UnitManager UnitManager;

    private Seeker Seeker;
    private Grid.Unit Unit;
	private MapGrid Grid;
    private int AttackRange;
    private int MoveRange;

    public void Awake() {
        Seeker = GetComponent<Seeker>();
        Unit = GetComponent<Grid.Unit>();
		Grid = CombatObjects.GetMap();
        AttackRange = 1;
        MoveRange = Unit.model.Character.Movement;
    }

    public IEnumerator act() {
        /**
         * Strategy:
         * 
         * - If we are adjacent to the target, HIT IT
         * - Get all squares you can move to using BFS
         * - Filter down to those from which you can hit your target
         * - If there are none, move as far toward your target as you can using A*
         * - If there are some, choose one at random, move to that square, then attack.
         */

        Vector2 targetLoc = Grid.GridPositionForWorldPosition(Target.transform.position);
        Vector2 ourGridPos = Unit.gridPosition;
        if (MathUtils.ManhattanDistance(targetLoc, ourGridPos) <= AttackRange) {
            yield return StartCoroutine(AttackTarget());
        } else {
            Vector2? attackPosition = FindAttackableLocation();
            if (attackPosition.HasValue) {
                yield return StartCoroutine(MoveThenAttack(attackPosition.Value));
            } else {
                yield return StartCoroutine(ApproachTarget());
            }
        }
    }

    private Vector2? FindAttackableLocation() {

        Vector2 targetLoc = Grid.GridPositionForWorldPosition(Target.transform.position);

        gameObject.SetActive(false);
        Grid.RescanGraph();

        Pathfinding.GraphNode occupiedNode = AstarPath.active.GetNearest(transform.position).node;
        List<Pathfinding.GraphNode> bfsResult = Pathfinding.PathUtilities.BFS(occupiedNode, MoveRange);

        gameObject.SetActive(true);
        Grid.RescanGraph();

        // Find the locations that are within move range
        List<Vector2> gridResults = (from node in bfsResult
                                     select Grid.GridPositionForWorldPosition((Vector3)node.position)).ToList();

        // Find the locations that are within attack range
        List<Vector2> attackableLocations = (from loc in gridResults
                                             where MathUtils.ManhattanDistance(loc, targetLoc) <= AttackRange
                                             select loc).ToList();
        if (attackableLocations.Any()) {
            int randomIndex = UnityEngine.Random.Range(0, attackableLocations.Count - 1);
            return attackableLocations[randomIndex];
        }

        return null;
    }

    private IEnumerator MoveThenAttack(Vector2 attackPosition) {
        Vector3 worldPos = Grid.GetWorldPosForGridPos(attackPosition);
        yield return StartCoroutine(ApproachPosition(worldPos));
        yield return StartCoroutine(AttackTarget());
    }

    private IEnumerator ApproachTarget() {
        yield return StartCoroutine(ApproachPosition(Target.transform.position));
    }
    private IEnumerator ApproachPosition(Vector3 position) {
        Pathfinding.Path path = null;
        Seeker.StartPath(transform.position, position, (p) => {
            path = p;
        });

        while (path == null) {
            yield return new WaitForEndOfFrame();
        }

		// Limit the path found to the unit's move range.
		int moveRange = Unit.model.Character.Movement;
		List<Vector3> limitedPath = path.vectorPath.Take(moveRange).ToList();

        if (!path.error) {
            yield return StartCoroutine(Unit.MoveAlongPath(limitedPath));

			Vector2 destination = Grid.GridPositionForWorldPosition(limitedPath.Last());
			UnitManager.ChangeUnitPosition(gameObject, destination);
			Grid.RescanGraph();
        } else {
            yield break;
        }
    }

    private IEnumerator AttackTarget() {
        Debug.Log("About to hit the target. Trust me.");
        yield return null;
    }
}