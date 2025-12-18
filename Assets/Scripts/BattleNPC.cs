using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BattleNPC : Entity
{
    public Button selectionButton;

    public static int[,] moveSelectionMatrix = {
        {0, 0, 0, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0}
    };

    void Update()
    {
        selectionButton.enabled = BattleInterface.active.targeting;
    }

    public static void UpdateMoveSelectionMatrix(Entity e) => moveSelectionMatrix[(int)e.state.hmHeuristic, (int)e.state.plannedMove]++;

    public Move SelectMove()
    {
        float[] probabilities = ComputeProbabilitiesForState(state.hmHeuristic);

        // Select random move weighted by selection data
        float r = (float)Utils.rand.NextDouble();
        int i;
        for (i = 0; i < probabilities.Length; i++)
        {
            r -= probabilities[i];
            if (r <= 0)
                break;
        }

        return (Move)i;
    }

    public float[] ComputeProbabilitiesForState(State state)
    {
        int numOptions = 4;
        // Get sum of entries in row
        int total = 0;
        for (int i = 0; i < numOptions; i++)
            total += moveSelectionMatrix[(int)state, i];

        float[] probs = new float[numOptions];

        // If no data, default to attack
        if (total == 0)
            probs[0] = 1;
        else
        {
            // Divide each entry by the sum to get the probability
            for (int i = 0; i < numOptions; i++)
                probs[i] = moveSelectionMatrix[(int)state, i] / (float)total;
        }

        return probs;
    }

    public void SelectTarget(List<Entity> targets)
    {
        state.target = targets[Random.Range(0, targets.Count)];
    }

    public void SelectThisAsTarget()
    {
        BattleInterface.active.SelectTarget(this);
    }
}