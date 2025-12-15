public abstract class BattleNPC : Entity
{
    public static int[,] moveSelectionMatrix = {
        {0, 0, 0, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0},
        {0, 0, 0, 0}
    };

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

        // Divide each entry by the sum to get the probability
        float[] probs = new float[numOptions];
        for (int i = 0; i < numOptions; i++)
            probs[i] = moveSelectionMatrix[(int)state, i] / (float)total;

        return probs;
    }
}