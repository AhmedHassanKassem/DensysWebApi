using System.Xml;
using Dapper;
using DensysWebApi.Entities;
using Microsoft.AspNetCore.Mvc;
using NewPointWebApi.Data;
using Newtonsoft.Json;


[Route("/api")]
[ApiController]
public class StaffController(ISqlDataAccess db, ILogger<StaffController> logger) : ControllerBase
{
    private readonly ISqlDataAccess _db = db;

    private readonly ILogger<StaffController> _logger = logger;
    [HttpGet("doctors")]

    public async Task<IActionResult> GetDoctors()
    {
        try
        {
            var conn = _db.OpenConnection();
            await conn.OpenAsync();

            string sql = @"SELECT  id_doctor,doctor_name,phone_number,address,specialization,remark,date_added FROM Doctors_Data";

            var result = conn.Query<DoctorsDatum>(sql).ToList();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = ex.Message }
            );
        }
    }

    [HttpPost("createDoctor")]
    public async Task<IActionResult> AddDoctor(DoctorsDatumDto doctor)
    {
        try
        {
            string sql = @"INSERT INTO Doctors_Data (doctor_name,phone_number,address,specialization,remark)           
                      VALUES(@doctor_name,@phone_number,@address,@specialization,@remark) ";
            await _db.SaveData(sql, new
            {
                doctor.doctor_name,
                doctor.phone_number,
                doctor.address,
                doctor.specialization,
                doctor.remark
            });
            return Ok(new { message = "Added successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError("Something went wrong");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    [HttpPatch("updateDoctor")]
    public async Task<IActionResult> UpdateDoctor(DoctorsDatumDto doctor, int docId)
    {
        try
        {
            string sql = @" UPDATE Doctors_Data SET    
                      doctor_name=@doctor_name,phone_number=@phone_number
                      ,address=@address,specialization=@specialization,remark=@remark WHERE id_doctor = @docId";
            await _db.SaveData(sql, new
            {
                docId,
                doctor.doctor_name,
                doctor.phone_number,
                doctor.address,
                doctor.specialization,
                doctor.remark
            });
            return Ok(new { message = "Updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something went wrong");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpDelete("deleteDoctor")]
    public async Task<IActionResult> DeleteDoctor(int docId)
    {
        try
        {
            string sql = @"DELETE FROM Doctors_Data WHERE id_doctor = @docId";
            await _db.LoadData<DoctorsDatum, dynamic>(sql, new { docId });
            return Ok("Deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting this doctor");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }


    //------------------------- Patients Apis ----------------------------//

    [HttpGet("patients")]
public async Task<IActionResult> GetPatients(string? search = "", int page = 0, int pageSize = 0)
{
    try
    {
        string sql = @"SELECT id_patient, id_doctor, patient_age, patient_gender, 
                        chronic_disease, patient_no, address, name, price, 
                        phone_1, phone_2, remark, date_added
                       FROM Patient_Data
                       WHERE name LIKE '%' + @Search + '%' 
                          OR patient_no LIKE '%' + @Search + '%'";

        if (page > 0 && pageSize > 0)
        {
            sql += " ORDER BY id_patient OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
        }

        var offset = (page - 1) * pageSize;

        string totalCount = @"SELECT COUNT(*) 
                              FROM Patient_Data 
                              WHERE name LIKE '%' + @Search + '%' 
                                 OR patient_no LIKE '%' + @Search + '%'";

        string lastPatientNumberQuery = @"SELECT TOP 1 patient_no 
                                          FROM Patient_Data 
                                          ORDER BY date_added DESC";

        using var conn = _db.OpenConnection();
        await conn.OpenAsync();

        // Get total count
        var totalItems = await conn.ExecuteScalarAsync<int>(totalCount, new { Search = search });
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        // Get paginated result
        var result = await conn.QueryAsync<PatientDatum>(
            sql,
            new { offset, pageSize, Search = search }
        );

        // Get last added patient number
        var lastAddedPatientNo = await conn.ExecuteScalarAsync<int>(lastPatientNumberQuery);

        var response = new
        {
            Patients = result,
            Pages = totalPages,
            LastAddedPatientNo = lastAddedPatientNo
        };

        return Ok(JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented));
    }
    catch (Exception ex)
    {
        _logger.LogInformation("Something went wrong", ex);
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
    }
}


 [HttpPost("createPatient")]
public async Task<IActionResult> CreatePatient(int docId, PatientDatumDto patient)
{
    try
    {
        string sql = @"INSERT INTO Patient_Data (id_doctor, patient_no, patient_age, patient_gender, 
                        chronic_disease, name, price, phone_1, phone_2, address, remark)
                        VALUES (@docId, @patient_no, @patient_age, @patient_gender, 
                        @chronic_disease, @name, @price, @phone_1, @phone_2, @address, @remark)";

        using var conn = _db.OpenConnection();
        await conn.OpenAsync();

        await _db.SaveData(sql, new
        {
            docId,
            patient.patient_no,
            patient.patient_age,  // patient_age should be here before name in the query
            patient.patient_gender,
            patient.chronic_disease,
            patient.name,          // name comes after chronic_disease in the query
            patient.price,
            patient.phone_1,
            patient.phone_2,
            patient.address,
            patient.remark
        });

        return Ok("Patient added successfully");
    }
    catch (Exception ex)
    {
        _logger.LogInformation("Something went wrong", ex);
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
    }
}

    [HttpPatch("updatePatient")]
    public async Task<IActionResult> UpdatePtient(int patientId, PatientDatumDto patient)
    {
        try
        {
            string sql = @"UPDATE Patient_Data SET id_doctor=@id_doctor, patient_no=@patient_no,
        name=@name,patient_age=@patient_age , patient_gender=@patient_gender , 
            chronic_disease=@chronic_disease, address=@address ,price=@price,phone_1=@phone_1 , phone_2=@phone_2 , remark=@remark WHERE id_patient = @patientId
        ";

            using var conn = _db.OpenConnection();
            await conn.OpenAsync();

            await _db.SaveData(sql, new
            {
                patientId,
                patient.id_doctor,
                patient.name,
                patient.patient_no,
                patient.patient_age,
                patient.patient_gender,
                patient.chronic_disease,
                patient.phone_1,
                patient.phone_2,
                patient.address,
                patient.price,
                patient.remark
            });

            return Ok("Patient updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }


    }
    [HttpDelete("deletePatient")]
    public async Task<IActionResult> DeletePatient(int patientId)
    {
        try
        {
            string sql = @"DELETE FROM Patient_Data WHERE id_patient=@patientId";
            using var conn = _db.OpenConnection();
            await conn.OpenAsync();
            await _db.LoadData<PatientDatum, dynamic>(sql, new { patientId });
            return Ok("Patient deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }


    //------------------------------- Proceds Apis ----------------------------//

    [HttpGet("proceds")]
    public async Task<IActionResult> GetProceds()
    {
        try
        {
            string sql = @"SELECT id_proced,proced_name,price,lab_cost , clinic_percent , doctor_percent ,remark     
            FROM Procedures_Data ";
            using var conn = _db.OpenConnection();
            await conn.OpenAsync();

            var result = conn.Query<ProceduresDatum>(sql).ToList();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    [HttpPost("createProceds")]
    public async Task<IActionResult> CreateProceds(ProceduresDatumDto proced)
    {
        try
        {
            string sql = @"INSERT INTO Procedures_Data (proced_name,price,lab_cost,doctor_percent,clinic_percent,remark)     
            VALUES (@proced_name,@price,@lab_cost,@doctor_percent,@clinic_percent,@remark)  ";
            using var conn = _db.OpenConnection();
            await conn.OpenAsync();

            await _db.SaveData(sql, new
            {
                proced.proced_name,
                proced.price,
                proced.lab_cost,
                proced.clinic_percent,
                proced.doctor_percent,
                proced.remark
            });
            return Ok("Procedure added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    [HttpPatch("updateProceds")]
    public async Task<IActionResult> UpdateProceds(int procedId, ProceduresDatumDto proced)
    {
        try
        {
            string sql = @"UPDATE Procedures_Data SET 
            proced_name=@proced_name,price=@price,lab_cost=@lab_cost,doctor_percent=@doctor_percent,clinic_percent=@clinic_percent
            ,remark=@remark WHERE id_proced=@procedId";

            using var conn = _db.OpenConnection();
            await conn.OpenAsync();

            await _db.SaveData(sql, new
            {
                procedId,
                proced.lab_cost,
                proced.clinic_percent,
                proced.doctor_percent,
                proced.price,
                proced.proced_name,
                proced.remark
            });
            return Ok("Procedure updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    [HttpDelete("deleteProced")]
    public async Task<IActionResult> DeleteProced(int procedId)
    {
        try
        {
            string sql = @"DELETE FROM Procedures_Data WHERE id_proced=@procedId";
            using var conn = _db.OpenConnection();
            await conn.OpenAsync();
            await _db.LoadData<ProceduresDatum, dynamic>(sql, new { procedId });
            return Ok("Procedure deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    [HttpGet("labs")]
    public async Task<IActionResult> GetLabs()
    {
        try
        {
            string sql = @"SELECT id_lab, id_proced,lab_name,lab_fee,remark     
            FROM Lab_Data ";
            using var conn = _db.OpenConnection();
            await conn.OpenAsync();
            var result = conn.Query<LabDatum>(sql).ToList();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    [HttpPost("createLabs")]
    public async Task<IActionResult> CreateLabs(int procedId, LabDatumDto lab)
    {
        try
        {
            string sql = @"INSERT INTO Lab_Data (id_proced,lab_name,lab_fee,remark)     
            VALUES (@procedId , @lab_name , @lab_fee , @remark)
            ";
            using var conn = _db.OpenConnection();
            await conn.OpenAsync();
            await _db.SaveData(sql, new
            {
                procedId,
                lab.lab_name,
                lab.lab_fee,
                lab.remark
            });

            return Ok("Lab added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    [HttpPatch("updateLabs")]
    public async Task<IActionResult> UpdateLabs(int labId, LabDatumDto lab)
    {
        try
        {
            string sql = @"UPDATE Lab_Data SET id_proced = @id_proced , lab_name=@lab_name , lab_fee=@lab_fee , remark=@remark 
            WHERE id_lab=@labId";
            using var conn = _db.OpenConnection();
            await conn.OpenAsync();
            await _db.SaveData(sql, new
            {
                labId,
                lab.id_proced,
                lab.lab_fee,
                lab.lab_name,
                lab.remark
            });
            return Ok("Lab updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpDelete("deleteLab")]

    public async Task<IActionResult> DeleteLab(int labId)
    {
        try
        {
            string sql = @"DELETE FROM Lab_Data WHERE id_lab=@labId";

            using var conn = _db.OpenConnection();
            await conn.OpenAsync();

            await _db.LoadData<LabDatum, dynamic>(sql, new { labId });

            return Ok("Lab deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong!", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            throw;
        }
    }


    //-------------------------------------- APPOINTMENTS --------------------------------------------//
  [HttpGet("appointment")]
public async Task<IActionResult> GetAppointment(int page = 0, int pageSize = 0, string? search = null)
{
    try
    {
        // Base SQL query
        string sql = @"
        SELECT ap.id_appoint, ap.id_patient, ap.id_doctor, ap.id_proced, 
               ap.appoint_date, ap.from_hour, ap.to_hour, ap.complete, 
               ap.amount_paid , ap.remaining_amount,
               ap.remark, ap.date_added, pd.name AS patient_name
        FROM Appointments AS ap
        JOIN Patient_Data AS pd ON ap.id_patient = pd.id_patient";

        // Add search condition if search term is provided
        if (!string.IsNullOrEmpty(search))
        {
            DateTime parsedDate;
            bool isDateSearch = DateTime.TryParse(search, out parsedDate);

            if (isDateSearch)
            {
                string formattedDate = parsedDate.ToString("yyyy-MM-dd");
                sql += @"
                WHERE 
                CONVERT(VARCHAR, appoint_date, 23) LIKE '%' + @Search + '%'";
            }
            else
            {
                sql += @"
                WHERE 
                (pd.name LIKE '%' + @Search + '%') 
                OR 
                (pd.patient_no LIKE '%' + @Search + '%')";
            }
        }

        // Only apply pagination if pageSize > 0
        if (pageSize > 0 && page > 0)
        {
            sql += " ORDER BY appoint_date OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
        }
        else
        {
            sql += " ORDER BY appoint_date"; // No pagination, return all records
        }

        var offset = (page - 1) * pageSize;

        using var conn = _db.OpenConnection();
        await conn.OpenAsync();

        // Execute the query
        var result = await conn.QueryAsync<Appointments>(
            sql,
            new { Search = search, offset, pageSize }
        );

        // Get the total count of records **only if pagination is used**
        int totalPages = 1;
        if (pageSize > 0)
        {
            string totalCountSql = @"
            SELECT COUNT(*) 
            FROM Appointments AS ap
            JOIN Patient_Data AS pd ON ap.id_patient = pd.id_patient";

            if (!string.IsNullOrEmpty(search))
            {
                DateTime parsedDate;
                bool isDateSearch = DateTime.TryParse(search, out parsedDate);

                if (isDateSearch)
                {
                    totalCountSql += @"
                    WHERE 
                    CONVERT(VARCHAR, appoint_date, 23) LIKE '%' + @Search + '%'";
                }
                else
                {
                    totalCountSql += @"
                    WHERE 
                    (pd.name LIKE '%' + @Search + '%') 
                    OR 
                    (pd.patient_no LIKE '%' + @Search + '%')";
                }
            }

            var totalItems = await conn.ExecuteScalarAsync<int>(totalCountSql, new { Search = search });
            totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        }

        var response = new
        {
            Appointments = result,
            Pages = totalPages
        };

        return Ok(response);
    }
    catch (Exception ex)
    {
        _logger.LogError("Something went wrong", ex);
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
    }
}


    [HttpPost("createApp")]
    public async Task<IActionResult> CreateAppoint(AppointmentsDto app)
    {
        try
        {
            string sql = @"INSERT INTO Appointments (id_patient,id_doctor,id_proced ,appoint_date,from_hour,to_hour , amount_paid , remaining_amount ,  remark)     
            VALUES (@id_patient,@id_doctor , @id_proced ,@appoint_date,@from_hour,@to_hour ,@amount_paid , @remaining_amount  , @remark)
            ";
            using var conn = _db.OpenConnection();
            await conn.OpenAsync();
            await _db.SaveData(sql, new
            {
                app.id_patient,
                app.id_doctor,
                app.id_proced,
                app.appoint_date,
                app.from_hour,
                app.to_hour,
                app.amount_paid,
                app.remaining_amount,
                app.remark
            });
            return Ok("Appointment added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    [HttpPatch("updateApp")]
    public async Task<IActionResult> UpdateAppoint(int appointId, AppointmentsDto app)
    {
        try
        {
            string sql = @"UPDATE Appointments SET id_patient=@id_patient,id_doctor=@id_doctor,id_proced=@id_proced,
            appoint_date=@appoint_date,from_hour=@from_hour,to_hour=@to_hour ,amount_paid=@amount_paid , remaining_amount=@remaining_amount , remark=@remark 
            WHERE id_appoint=@appointId";
            using var conn = _db.OpenConnection();
            await conn.OpenAsync();
            await _db.SaveData(sql, new
            {
                appointId,
                app.id_patient,
                app.id_doctor,
                app.id_proced,
                app.appoint_date,
                app.from_hour,
                app.to_hour,
                app.amount_paid,
                app.remaining_amount,
                app.remark
            });
            return Ok("Appointment updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpDelete("deleteApp")]

    public async Task<IActionResult> DeleteAppoint(int appointId)
    {
        try
        {
            string sql = @"DELETE FROM Appointments WHERE id_appoint=@appointId";

            using var conn = _db.OpenConnection();
            await conn.OpenAsync();

            await _db.LoadData<Appointments, dynamic>(sql, new { appointId });

            return Ok("Appointment deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong!", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            throw;
        }
    }





    [HttpGet("labCases")]
    public async Task<IActionResult> GetLabCases()
    {
        try
        {
            string sql = @"SELECT id_case,  id_lab, id_doctor,id_patient , id_lab, id_proced ,price ,remark , date_added     
            FROM Lab_Cases";
            using var conn = _db.OpenConnection();
            await conn.OpenAsync();
            var result = conn.Query<LabCases>(sql).ToList();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    [HttpPost("createLabCase")]
    public async Task<IActionResult> CreateCases(int patId , int docId , int labId, int procedId , LabCasesDto lab)
    {
        try
        {
            string sql = @"INSERT INTO Lab_Cases (id_doctor , id_patient , id_lab , price , id_proced,remark)     
            VALUES (@docId , @patId , @labId , @price , @procedId , @remark)
            ";
            using var conn = _db.OpenConnection();
            await conn.OpenAsync();
            await _db.SaveData(sql, new
            {
                patId,
                docId,
                labId,
                procedId,
                lab.price,
                lab.remark
            });

            return Ok("Lab added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    [HttpPatch("updateLabCases")]
    public async Task<IActionResult> UpdateCases(int caseId, LabCasesDto lab)
    {
        try
        {
            string sql = @"UPDATE Lab_Cases SET id_doctor=@id_doctor , id_patient=@id_patient , id_lab=@id_lab , price=@price 
            , id_proced=@id_proced,remark=@remark
            WHERE id_case=@caseId";
            using var conn = _db.OpenConnection();
            await conn.OpenAsync();
            await _db.SaveData(sql, new
            {
                caseId,
                lab.id_doctor,
                lab.id_lab,
                lab.id_patient,
                lab.price,
                lab.id_proced,
                lab.remark
            });
            return Ok("Lab updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpDelete("deleteLabCases")]

    public async Task<IActionResult> DeleteCases(int caseId)
    {
        try
        {
            string sql = @"DELETE FROM Lab_Cases WHERE id_case=@caseId";

            using var conn = _db.OpenConnection();
            await conn.OpenAsync();

            await _db.LoadData<LabCases, dynamic>(sql, new { caseId });

            return Ok("Lab deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong!", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            throw;
        }
    }

}