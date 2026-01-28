using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/documents")]
    public class DocumentsController : ControllerBase
    {
        private readonly DocumentService _documentService;

        public DocumentsController(DocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult Create([FromForm] UploadDocumentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.DocumentName))
                return BadRequest("Document name is required");

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("File is required");

            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );

            var document = _documentService.CreateDocumentWithFile(dto, userId);

            return Ok(new
            {
                document.DocId,
                document.DocumentName,
                document.DocumentType,
                document.CreatedOn
            });
        }

        // ===============================
        // GET USER DOCUMENTS
        // ===============================
        [HttpGet]
        public IActionResult GetMyDocuments()
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );

            var documents = _documentService.GetUserDocuments(userId)
                .Select(d => new DocumentResponseDto
                {
                    DocId = d.DocId,
                    DocumentName = d.DocumentName,
                    DocumentDescription = d.DocumentDescription,
                    DocumentType = d.DocumentType,
                    CreatedOn = d.CreatedOn
                });

            return Ok(documents);
        }

        // ===============================
        // GET DOCUMENT BY ID
        // ===============================
        [HttpGet("{docId}")]
        public IActionResult GetById(int docId)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );

            var document = _documentService.GetDocumentById(docId, userId);

            if (document == null)
                return NotFound("Document not found or access denied");

            return Ok(new DocumentResponseDto
            {
                DocId = document.DocId,
                DocumentName = document.DocumentName,
                DocumentDescription = document.DocumentDescription,
                DocumentType = document.DocumentType,
                CreatedOn = document.CreatedOn
            });
        }
    }
}
