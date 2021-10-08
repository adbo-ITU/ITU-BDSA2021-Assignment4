using System.Collections.Generic;
using System.IO;
using Assignment4.Core;
using Microsoft.Extensions.Configuration;
using System.Linq;

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
            var existingTagWithName = from t in _context.Tags
                                      where t.Name == tag.Name
                                      select t.Id;

            if (existingTagWithName.Any())
                return (Response.Conflict, existingTagWithName.First());

            var newTag = new Tag { Name = tag.Name };
            _context.Tags.Add(newTag);
            _context.SaveChanges();

            return (Response.Created, newTag.Id);
        }

        public Response Delete(int tagId, bool force = false)
        {
            var tag = _context.Tags.Find(tagId);

            if (tag == null) return Response.NotFound;
            if (tag.tasks.Count > 0 && !force) return Response.Conflict;

            _context.Tags.Remove(tag);

            return Response.Deleted;
        }

        public TagDTO Read(int tagId)
        {
            var tag = _context.Tags.Find(tagId);
            return tag != null ? new TagDTO(tag.Id, tag.Name) : null;
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
