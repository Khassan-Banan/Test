namespace Guide.Client.Services
{
    public class TrackResult
    {
        public TrackResult(float score)
        {
            Score = score;
        }

        public TrackResult(string message)
        {
            Message = message;
        }

        public float? Score { get; }

        /// <summary>
        /// Explanation in case Score is null.
        /// </summary>
        /// <remarks>
        /// This happens with failed foundational courses or missing courses of the required type.
        /// </remarks>
        public string Message { get; }
    }
}