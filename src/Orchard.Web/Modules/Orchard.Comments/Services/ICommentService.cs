using Orchard.Comments.Models;
using Orchard.ContentManagement;

namespace Orchard.Comments.Services {
    public interface ICommentService : IDependency {
        IContentQuery<CommentPart, CommentPartRecord> GetComments();
        IContentQuery<CommentPart, CommentPartRecord> GetComments(CommentStatus status);
        IContentQuery<CommentPart, CommentPartRecord> GetCommentsForCommentedContent(int id);
        IContentQuery<CommentPart, CommentPartRecord> GetCommentsForCommentedContent(int id, CommentStatus status);
        CommentPart GetComment(int id);
        ContentItemMetadata GetDisplayForCommentedContent(int id);
        ContentItem GetCommentedContent(int id);
        void ApproveComment(int commentId);
        void UnapproveComment(int commentId);
        void DeleteComment(int commentId);
        bool CommentsDisabledForCommentedContent(int id);
        void DisableCommentsForCommentedContent(int id);
        void EnableCommentsForCommentedContent(int id);
    }
}