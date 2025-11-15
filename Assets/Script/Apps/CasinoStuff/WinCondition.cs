 [System.Serializable]
    public class WinCondition
    {
        public string name;
        public int requiredCount = 3;
        public int targetNumber = -1;
        public int payoutMultiplier = 1;
        public string resultMessage = "Win!";

        public bool IsMatch(int[] reels)
        {
            if (requiredCount == 3)
            {
                bool allMatch = reels[0] == reels[1] && reels[1] == reels[2];
                if (!allMatch) return false;
                return targetNumber == -1 || reels[0] == targetNumber;
            }
            else if (requiredCount == 2)
            {
                bool hasPair = 
                    (reels[0] == reels[1]) ||
                    (reels[0] == reels[2]) ||
                    (reels[1] == reels[2]);
                if (!hasPair) return false;
                if (targetNumber != -1)
                {
                    return reels[0] == targetNumber || 
                           reels[1] == targetNumber || 
                           reels[2] == targetNumber;
                }
                return true;
            }
            return false;
        }
    }
    
