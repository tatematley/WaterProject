using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WaterProject.API.data;

namespace WaterProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class WaterController : ControllerBase
    {
        private WaterDbContext _waterContext;
        
        public WaterController(WaterDbContext temp) => _waterContext = temp;

        [HttpGet("AllProjects")]
        public IActionResult GetProjects(int pageSize = 10, int pageNum = 1, [FromQuery] List<string>? projectTypes = null)
        {
            IQueryable query = _waterContext.Projects.AsQueryable();

            if (projectTypes != null && projectTypes.Any())
            {
                query = query.Where(p => projectTypes.Contains(p.ProjectType));
            }
            
            // GET the cookie to the console (can see in rider terminal)  
            string? favProjType = Request.Cookies["FavoriteProjectType"];
            Console.WriteLine("--------COOKIE-------\n" + favProjType);
            
            
            // Create a COOKIE 
            HttpContext.Response.Cookies.Append("FavoriteProjectType", "Borehole Well adn Hand Pump", new CookieOptions
            {
                HttpOnly = true, // cookie is only seen by the server
                Secure = true, // cookie only transmitted over https 
                SameSite = SameSiteMode.Strict, // limits cookies from other sites (strict if we want it to be secure) 
                Expires = DateTime.Now.AddMinutes(1) // when we want the session cookie to expire 
            });
            
            // num of projects
            var totalNumProjects = query.Count();
            
            // projects 
            var something = query
                .Skip((pageNum-1) * pageSize)
                .Take(pageSize)
                .ToList();

            // makes it so you can return both the num of projects and the Projects 
            var someObject = new
            {
                Projects = something,
                TotalNumProjects = totalNumProjects
            };
            return Ok(someObject); // this OK makes it so it gets a 200 and passes through
        }

        [HttpGet("GetProjectTypes")]
        public IActionResult GetProjectTypes()
        {
            var projectTypes = _waterContext.Projects
                .Select(p => p.ProjectType )
                .Distinct()
                .ToList();
            
            return Ok(projectTypes);
        }

        // Add 
        [HttpPost("AddProject")]
        public IActionResult AddProject([FromBody] Project newProject)
        {
            _waterContext.Projects.Add(newProject);
            _waterContext.SaveChanges();
            return Ok(newProject);
        }


        // Update
        [HttpPut("UpdateProject/{projectId}")]
        public IActionResult UpdateProject(int projectId, [FromBody] Project updatedProject)
        {
            var existingProject = _waterContext.Projects.Find(projectId);
            

            existingProject.ProjectName = updatedProject.ProjectName;
            existingProject.ProjectType = updatedProject.ProjectType;
            existingProject.ProjectRegionalProgram = updatedProject.ProjectRegionalProgram;
            existingProject.ProjectImpact = updatedProject.ProjectImpact;
            existingProject.ProjectPhase = updatedProject.ProjectPhase;
            existingProject.ProjectFunctionalityStatus = updatedProject.ProjectFunctionalityStatus;

            _waterContext.Projects.Update(existingProject);
            _waterContext.SaveChanges();

            return Ok(existingProject);
        }

        // Delete
        [HttpDelete("DeleteProject/{projectId}")]
        public IActionResult DeleteProject(int projectId) 
        {
            var project = _waterContext.Projects.Find(projectId);

            if (project == null) 
            {
                reutnr NOtFound(new {message = "Project not found"});
            }
        }
    }
}