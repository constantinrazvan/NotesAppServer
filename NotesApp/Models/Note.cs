using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string BodyText { get; set; }
    }

    public class NoteDTO()
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string BodyText { get; set; }
    }
}
