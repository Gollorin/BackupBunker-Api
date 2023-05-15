using backup_bunker_api.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace backup_bunker_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupBunkerController : ControllerBase
    {
        private FirestoreDb firebase_db = FireStoreManager.Firestore_DB;
        LogManager LogMn = new LogManager();


        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(InputLogin login)
        {
            CollectionReference users_ref = firebase_db.Collection("tbUsers");

            Query query = users_ref.WhereEqualTo("Email", login.email);

            QuerySnapshot user_snap = await query.GetSnapshotAsync();

            string hash = user_snap[0].GetValue<string>("Password");

            string? respornse_value = BCrypt.Net.BCrypt.Verify(login.password, hash) ? user_snap[0].Id : null;

            string response = JsonConvert.SerializeObject(new { Id = respornse_value });

            LogMn.WriteMessage($"User {login.email} loged in!");

            if (BCrypt.Net.BCrypt.Verify(login.password, hash))
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }


        [HttpGet("find-backups-by-id/{id}")]
        public async Task<IActionResult> GetBackupsByIdAsync(string id)
        {
            DocumentReference schedule_ref = firebase_db.Collection("tbSchedule").Document(id);

            DocumentSnapshot schedule_data = await schedule_ref.GetSnapshotAsync();

            List<string> all_backups_id = schedule_data.GetValue<List<string>>("Backups");

            List<ItemBackup> all_backups = new List<ItemBackup>();

            foreach (string back_id in all_backups_id)
            {
                DocumentReference backup_ref = firebase_db.Collection("tbBackups").Document(back_id);

                DocumentSnapshot backup_snap = await backup_ref.GetSnapshotAsync();

                if (backup_snap.Exists)
                {
                    ItemBackup temp_backup = new ItemBackup()
                    {
                        Id = backup_snap.Id,
                        Name = backup_snap.GetValue<string>("Name"),
                        Paths_From = backup_snap.GetValue<List<string>>("Paths_From"),
                        Paths_To = backup_snap.GetValue<List<string>>("Paths_To"),
                        OnDaysOfWeek = backup_snap.GetValue<List<bool>>("OnDaysOfWeek"),
                        AtTimes = backup_snap.GetValue<List<string>>("AtTimes"),
                        Type = backup_snap.GetValue<string>("Type"),
                        CreateAt = backup_snap.GetValue<string>("CreateAt"),
                    };

                    all_backups.Add(temp_backup);
                }
            }

            string response = JsonConvert.SerializeObject(all_backups);

            LogMn.WriteMessage($"Someone asked for backups!");

            return Ok(response);
        }
    }
}
