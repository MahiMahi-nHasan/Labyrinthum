using UnityEngine;

public class BattleNPC : BattleEntity
{
    public static int[,] moveSelectionMatrix = {
        {0, 0, 0, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0}
    };

    public static void UpdateMoveSelectionMatrix(BattleEntity e) => moveSelectionMatrix[(int)e.state.hmHeuristic, (int)e.state.plannedMove]++;

    public Move GetDecidedMove()
    {
        Debug.Log("Enemy is selecting move");
        
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

        Debug.Log("Enemy selected " + (Move)i);

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
}