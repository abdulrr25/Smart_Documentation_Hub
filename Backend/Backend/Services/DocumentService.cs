using Backend.Data;
using Backend.DTOs;
using Backend.Models;

namespace Backend.Services
{
    public class DocumentService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DocumentService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ✅ Existing method (UNCHANGED)
        public Document CreateDocument(UploadDocumentDto dto, int userId)
        {
            var document = new Document
            {
                DocumentName = dto.DocumentName,
                DocumentDescription = dto.DocumentDescription,
                DocumentType = dto.DocumentType,
                UserId = userId,
                CreatedOn = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            _context.SaveChanges();

            _context.ActivityLogs.Add(new ActivityLog
            {
                UserId = userId,
                Action = "Document Created",
                EntityName = "Document",
                EntityId = document.DocId,
                CreatedAt = DateTime.UtcNow
            });

            _context.SaveChanges();
            return document;
        }

        // ✅ CREATE DOCUMENT + SAVE FILE LOCALLY (FIXED)
        public Document CreateDocumentWithFile(UploadDocumentDto dto, int userId)
        {
            var document = new Document
            {
                DocumentName = dto.DocumentName,
                DocumentDescription = dto.DocumentDescription,
                DocumentType = dto.DocumentType,
                UserId = userId,
                CreatedOn = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            _context.SaveChanges(); // generates DocId

            // ===============================
            // 📁 Safe local file storage
            // ===============================
            var uploadsRoot = Path.Combine(_env.ContentRootPath, "Uploads");
            var userFolder = Path.Combine(uploadsRoot, $"user-{userId}");
            var docFolder = Path.Combine(userFolder, $"doc-{document.DocId}");

            Directory.CreateDirectory(docFolder);

            // 🔐 Safe + unique file name
            var safeFileName =
                $"{Guid.NewGuid()}{Path.GetExtension(dto.File.FileName)}";

            var filePath = Path.Combine(docFolder, safeFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                dto.File.CopyTo(stream);
            }

            // ===============================
            // 💾 Save file path in DB
            // ===============================
            document.FilePath =
                $"Uploads/user-{userId}/doc-{document.DocId}/{safeFileName}";

            _context.SaveChanges();

            // ===============================
            // 📝 Activity log
            // ===============================
            _context.ActivityLogs.Add(new ActivityLog
            {
                UserId = userId,
                Action = "Document Uploaded",
                EntityName = "Document",
                EntityId = document.DocId,
                CreatedAt = DateTime.UtcNow
            });

            _context.SaveChanges();

            return document;
        }

        public List<Document> GetUserDocuments(int userId)
        {
            return _context.Documents
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedOn)
                .ToList();
        }

        public Document? GetDocumentById(int docId, int userId)
        {
            return _context.Documents
                .FirstOrDefault(d => d.DocId == docId && d.UserId == userId);
        }
    }
}
