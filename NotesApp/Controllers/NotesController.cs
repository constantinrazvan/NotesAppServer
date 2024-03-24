using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using NotesApp.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using static MongoDB.Driver.WriteConcern;

namespace NotesApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotesController(MongoDBContext mongoDBContext) : ControllerBase
    {

        private readonly IMongoDatabase _database = mongoDBContext.Database;

        [HttpPost("/notes/newNote")]
        public async Task<ActionResult> NewNote([FromBody] Note note)
        {
            if (note == null)
            {
                return BadRequest("You cannot upload an empty note!");
            }

            var notes = _database.GetCollection<Note>("notes");

            await notes.InsertOneAsync(note);

            return Ok($"The note with ID {note.Id} created!");
        }

        [HttpGet("/notes")]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
        {
            try
            {
                var _notesCollection = _database.GetCollection<Note>("notes");
                var filter = Builders<Note>.Filter.Empty;
                var notes = await _notesCollection.Find(filter).ToListAsync();

                return Ok(notes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("/notes/{id}")]
        public async Task<ActionResult<Note>> DeleteNote(string id)
        {
            try
            {
                var _notes = _database.GetCollection<Note>("notes");
                var filter = Builders<Note>.Filter.Eq("_id", ObjectId.Parse(id));

                await _notes.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Note deleted!");
        }

        [HttpPut("/notes/{id}")]
        public async Task<ActionResult<Note>> UpdateNote(string id, Note note)
        {
            try
            {
                var _notes = _database.GetCollection<Note>("notes");

                var filter = Builders<Note>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<Note>.Update
                    .Set(n => n.Title, note.Title) 
                    .Set(n => n.BodyText, note.BodyText);

                var options = new FindOneAndUpdateOptions<Note>
                {
                    ReturnDocument = ReturnDocument.After
                };

                if (note == null)
                {
                    return BadRequest();
                }
                else
                {
                    var updatedNote = await _notes.FindOneAndUpdateAsync(filter, update, options);
                    if (updatedNote == null)
                    {
                        return NotFound(); 
                    }
                    return Ok(updatedNote);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
