using System.Collections.Generic;
using System.IO;
using Assignment4.Core;
using Microsoft.Extensions.Configuration;

namespace Assignment4.Entities
{
    public class TagRepository : ITagRepository
    {
        private KanbanContext _context;

        public TagRepository(KanbanContext context)
        {
            _context = context;
        }

        public (Response Response, int TagId) Create(TagCreateDTO tag)
        {
            throw new System.NotImplementedException();
        }

        public Response Delete(int tagId, bool force = false)
        {
            var tag = _context.Tags.Find(tagId);

            if (tag.tasks.Count > 0 && !force)
            {
                return Response.Conflict;
            }

            _context.Tags.Remove(tag);

            return Response.Deleted;
        }

        public TagDTO Read(int tagId)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<TagDTO> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public Response Update(TagUpdateDTO tag)
        {
            throw new System.NotImplementedException();
        }
    }
}
