using student_profile.BLL.Interfaces;
using student_profile.Data.Context;
using student_profile.Data.Models;

namespace student_profile.BLL.Repositories;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(AppDbContext context) : base(context)
    {
    }
}

