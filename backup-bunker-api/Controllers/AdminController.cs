using backup_bunker_api.Models;
using BCrypt.Net;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace backup_bunker_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private FirestoreDb firebase_db = FireStoreManager.Firestore_DB;
        private LogManager LogMn = new LogManager();

        #region USERS METHODS

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


        private string HashPassword(string raw_password)
        {
            return BCrypt.Net.BCrypt.HashPassword(raw_password);
        }


        [HttpPost("create-user")]
        public async Task<IActionResult> NewUser(InputUser bodyUser)
        {
            CollectionReference users_ref = firebase_db.Collection("tbUsers");

            string now_time = DateTime.Now.ToString();

            Dictionary<string, object> user = new Dictionary<string, object>()
            {
                { "Email", bodyUser.Email },
                { "Password", HashPassword(bodyUser.Password) },
                { "CreateAt", now_time },
            };

            DocumentReference user_ref = await users_ref.AddAsync(user);

            DocumentReference schedule_ref = firebase_db.Collection("tbSchedule").Document(user_ref.Id);

            Dictionary<string, object> anonym = new Dictionary<string, object>()
            {
                { "Backups", new List<string>() },
            };

            await schedule_ref.SetAsync(anonym);

            LogMn.WriteMessage($"User with email ({bodyUser.Email}) was created");

            return Ok(JsonConvert.SerializeObject(new { Email = bodyUser.Email }));
        }


        [HttpGet("find-by-id/{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            DocumentReference user_ref = firebase_db.Collection("tbUsers").Document(id);

            DocumentSnapshot user_snap = await user_ref.GetSnapshotAsync();

            if (user_snap.Exists)
            {
                ItemUser user = new ItemUser()
                {
                    Id = user_snap.Id,
                    Email = user_snap.GetValue<string>("Email"),
                    Password = null,
                    CreateAt = user_snap.GetValue<string>("CreateAt"),
                };

                string response = JsonConvert.SerializeObject(user);

                return Ok(response);
            } else
            {
                return BadRequest();
            }
        }


        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            CollectionReference users_ref = firebase_db.Collection("tbUsers");

            QuerySnapshot query_snap = await users_ref.GetSnapshotAsync();

            List<ItemUser> all_users = new List<ItemUser>();

            foreach (DocumentSnapshot user in query_snap.Documents)
            {
                ItemUser temp_user = new ItemUser()
                {
                    Id = user.Id,
                    Email = user.GetValue<string>("Email"),
                    Password = null,
                    CreateAt = user.GetValue<string>("CreateAt"),
                };

                all_users.Add(temp_user);
            }

            string response = JsonConvert.SerializeObject(all_users);

            return Ok(response);
        }


        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUserById(string id)
        {
            DocumentReference docRef = firebase_db.Collection("tbUsers").Document(id);
            await docRef.DeleteAsync();

            DocumentReference schRef = firebase_db.Collection("tbSchedule").Document(id);
            await schRef.DeleteAsync();

            string response = JsonConvert.SerializeObject(id);

            LogMn.WriteMessage($"One of the user was deleted!");

            return Ok(response);
        }

        #endregion

        #region BACKUPS METHOD
        [HttpPost("create-backup")]
        public async Task<IActionResult> NewBackup(InputBackup bodyBackup)
        {
            Debug.WriteLine(bodyBackup.Name);

            CollectionReference backups_ref = firebase_db.Collection("tbBackups");

            Dictionary<string, object> backup = new Dictionary<string, object>()
            {
                { "Name", bodyBackup.Name },
                { "Paths_From", bodyBackup.Paths_From },
                { "Paths_To", bodyBackup.Paths_To },
                { "OnDaysOfWeek", bodyBackup.OnDaysOfWeek },
                { "AtTimes", UzMeToNebavi(bodyBackup.AtTimes) },
                { "Type", bodyBackup.Type },
                { "CreateAt", DateTime.Now.ToString() },
            };

            await backups_ref.AddAsync(backup);

            string response = JsonConvert.SerializeObject(bodyBackup.Name);

            LogMn.WriteMessage($"Backup {bodyBackup.Name} was created");

            return Ok(response);
        }


        public static string[] UzMeToNebavi(List<string> list)
        {
            string[] result = new string[list.Count]; 

            for (int i = 0; i < list.Count; i++)
            {
                TimeOnly timeOnly = TimeOnly.ParseExact(list[i], "HH:mm", CultureInfo.InvariantCulture);
                result[i] = timeOnly.ToString();
            }

            return result;
        }


        [HttpGet("get-all-backups")]
        public async Task<IActionResult> GetAllBackupsAsync()
        {
            CollectionReference backups_ref = firebase_db.Collection("tbBackups");

            QuerySnapshot query_snap = await backups_ref.GetSnapshotAsync();

            List<ItemBackup> all_backups = new List<ItemBackup>();

            foreach (DocumentSnapshot backup in query_snap.Documents)
            {
                ItemBackup temp_backup = new ItemBackup()
                {
                    Id = backup.Id,
                    Name = backup.GetValue<string>("Name"),
                    Type = backup.GetValue<string>("Type"),
                    Paths_From = backup.GetValue<List<string>>("Paths_From"),
                    Paths_To = backup.GetValue<List<string>>("Paths_To"),
                    OnDaysOfWeek = backup.GetValue<List<bool>>("OnDaysOfWeek"),
                    AtTimes = backup.GetValue<List<string>>("AtTimes"),
                    CreateAt = backup.GetValue<string>("CreateAt"),
                };

                all_backups.Add(temp_backup);
            }

            string response = JsonConvert.SerializeObject(all_backups);

            return Ok(response);
        }


        [HttpGet("find-backup-by-id/{id}")]
        public async Task<ItemBackup> GetBackupByIdAsync(string id)
        {
            DocumentReference backup_ref = firebase_db.Collection("tbBackups").Document(id);

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

                return temp_backup;
            }
            else
            {
                return null;
            }
        }


        [HttpDelete("delete-backup/{id}")]
        public async Task<IActionResult> DeleteBackupById(string id)
        {
            DocumentReference docRef = firebase_db.Collection("tbBackups").Document(id);
            await docRef.DeleteAsync();

            string response = JsonConvert.SerializeObject(id);

            LogMn.WriteMessage($"One of the backups was deleted!");

            return Ok(response);
        }
        
        #endregion

        #region SCHEDULE METHODS

        [HttpPost("create-schedule")]
        public async Task<IActionResult> NewSchedule(InputSchedule bodySchedule)
        {
            DocumentReference schedule_ref = firebase_db.Collection("tbSchedule").Document(bodySchedule.Id);

            Dictionary<string, object> schedule = new Dictionary<string, object>()
            {
                { "Backups", bodySchedule.Backups },
            };

            await schedule_ref.SetAsync(schedule);

            string response = JsonConvert.SerializeObject(bodySchedule.Id);

            LogMn.WriteMessage($"Schedule was created!");

            return Ok(response);
        }


        [HttpGet("find-backups-ids-by-id/{id}")]
        public async Task<IActionResult> GetBackupsIdsByIdAsync(string id)
        {
            DocumentReference schedule_ref = firebase_db.Collection("tbSchedule").Document(id);

            DocumentSnapshot schedule_data = await schedule_ref.GetSnapshotAsync();

            List<string> all_backups_id = schedule_data.GetValue<List<string>>("Backups");

            string response = JsonConvert.SerializeObject(all_backups_id);

            return Ok(response);
        }

        #endregion

        #region LOG METHOD
        [HttpGet("get-log")]
        public async Task<IActionResult> GetLog()
        {
            List<string> log_mess = LogMn.ReadLastHundredMessage();
            
            string response = JsonConvert.SerializeObject(log_mess);

            return Ok(response);
        }
        #endregion
    }
}
