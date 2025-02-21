namespace Roo.Azure.Configuration.Common.Models
{
    public class UserInfo
    {
        /// <summary>
        /// Gets/Sets the username for the current user using.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets/Sets the email attached to the current user.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets/Sets the userId attached to the current user.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Gets/Sets whether the user is authenticated.
        /// </summary>
        public bool IsAuthenticated { get; set; } = false;

        /// <summary>
        /// Whether this model has any info in it.
        /// </summary>
        public bool HasInfo
        {
            get => !string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Email) || !string.IsNullOrEmpty(UserId);
        }
    }
}
