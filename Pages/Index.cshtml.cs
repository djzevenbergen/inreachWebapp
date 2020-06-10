using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Threading.Tasks;
namespace InreachWebapp.Pages
{
  public class UploadFileModel : PageModel
  {
    private IHostingEnvironment _environment;
    public UploadFileModel(IHostingEnvironment environment)
    {
      _environment = environment;
    }
    [BindProperty]
    public IFormFile Upload { get; set; }
    public async Task OnPostAsync()
    {
      var file = Path.Combine(_environment.ContentRootPath, "uploads", Upload.FileName);
      using (var fileStream = new FileStream(file, FileMode.Create))
      {
        await Upload.CopyToAsync(fileStream);
      }
    }
  }
}