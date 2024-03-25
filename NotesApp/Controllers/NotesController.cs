using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using NotesApp.Models;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace NotesApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotesController : ControllerBase
    {

        private readonly string connectionString;

        public NotesController(IConfiguration configuration) : base()
        {
            connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
        }

        [HttpGet("/notes")]
        public ActionResult GetNotes()
        {
            List<Note> notes = new List<Note>();

            try
            {
                using (var connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();
                    var sql = "SELECT * FROM notes";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Note note = new Note();

                                note.Id = reader.GetInt32(0);
                                note.Title = reader.GetString(1);

                                notes.Add(note);
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("/notes/newNote")]
        public ActionResult CreateNote(NoteDTO noteDTO)
        {
            int i = 1;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "INSERT INTO notes " +
                                "(title, bodyText) VALUES" +
                                "(@title, @bodyText)";

                    if (noteDTO.Title.Length == 0 || noteDTO.BodyText.Length == 0)
                    {
                        ModelState.AddModelError("Note", "Sorry but you have to fill all fields!");
                        return StatusCode(400, ModelState);
                    }

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", noteDTO.Title);
                        command.Parameters.AddWithValue("@bodyText", noteDTO.BodyText);
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            i++;
            return StatusCode(200, $"Note with id {i} succesfully created!");
        }

        [HttpPatch("/notes/update/{id}")]
        public ActionResult UpdateNote(NoteDTO noteDTO, int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "UPDATE notes " +
                                 "SET title = @title, bodyText = @bodyText " +
                                 "WHERE Noteid = @id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", noteDTO.Title);
                        command.Parameters.AddWithValue("@bodyText", noteDTO.BodyText);
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }


        [HttpDelete("/notes/delete/{id}")]
        public ActionResult DeleteNote(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var sql = "DELETE FROM notes WHERE id-@id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

            } catch(Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
            return Ok($"Note with id {id} has been removed!");
        }

    }
}
