import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import "../styles/EditDocument.css";
import api from "../services/api";

export default function EditDocument() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [viewMode, setViewMode] = useState(false);
  const [content, setContent] = useState("");
  const [latestVersion, setLatestVersion] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const [comments, setComments] = useState([]);
  const [selectionRange, setSelectionRange] = useState(null);
  const [newComment, setNewComment] = useState("");

  // ==========================
  // LOAD DOCUMENT + COMMENTS
  // ==========================
  useEffect(() => {
    if (!id) return;

    async function loadLatest() {
      try {
        setLoading(true);

        const res = await api(`/api/documents/${id}/versions/latest`);
        if (!res) return;

        setLatestVersion(res);
        setContent(res.originalText || "");

        const commentRes = await api(
          `/api/inline-comments/version/${res.versionId}`
        );

        setComments(commentRes || []);
      } catch (err) {
        console.error(err);
        alert("Failed to load document");
      } finally {
        setLoading(false);
      }
    }

    loadLatest();
  }, [id]);

  // ==========================
  // SAVE NEW VERSION
  // ==========================
  async function saveChanges() {
    if (!content.trim()) return alert("Content cannot be empty");

    try {
      setSaving(true);

      await api(`/api/documents/${id}/versions`, {
        method: "POST",
        body: JSON.stringify({ text: content }),
      });

      alert("Saved! New version created.");
    } catch (err) {
      console.error(err);
      alert("Save failed");
    } finally {
      setSaving(false);
    }
  }

  // ==========================
  // TEXT SELECTION HANDLER
  // ==========================
  function handleTextSelection() {
    if (!viewMode) return;

    const selection = window.getSelection();
    if (!selection || selection.rangeCount === 0) return;

    const range = selection.getRangeAt(0);
    const container = document.querySelector(".document-view");
    if (!container) return;

    if (!container.contains(range.startContainer)) return;

    const preSelectionRange = range.cloneRange();
    preSelectionRange.selectNodeContents(container);
    preSelectionRange.setEnd(range.startContainer, range.startOffset);

    const start = preSelectionRange.toString().length;
    const end = start + range.toString().length;

    if (start >= end) return;

    setSelectionRange({ start, end });
  }

  // ==========================
  // ADD INLINE COMMENT
  // ==========================
  async function addComment() {
    if (!newComment.trim() || !selectionRange) return;

    try {
      const res = await api("/api/inline-comments", {
        method: "POST",
        body: JSON.stringify({
          versionId: latestVersion.versionId,
          startIndex: selectionRange.start,
          endIndex: selectionRange.end,
          commentText: newComment,
        }),
      });

      setComments((prev) => [...prev, res]);
      setNewComment("");
      setSelectionRange(null);
      window.getSelection().removeAllRanges();
    } catch (err) {
      console.error(err);
      alert("Failed to add comment");
    }
  }

  // ==========================
  // APPLY HIGHLIGHTS
  // ==========================
  function applyHighlights(text, comments) {
    if (!comments.length) return text;

    let updatedText = text;

    const sorted = [...comments].sort(
      (a, b) => b.startIndex - a.startIndex
    );

    for (const c of sorted) {
      updatedText =
        updatedText.slice(0, c.startIndex) +
        `<span 
          class="highlight"
          data-id="${c.commentId}"
          id="highlight-${c.commentId}"
        >
          ${updatedText.slice(c.startIndex, c.endIndex)}
        </span>` +
        updatedText.slice(c.endIndex);
    }

    return updatedText;
  }

  const highlightedContent = applyHighlights(content, comments);

  // ==========================
  // CLICK HIGHLIGHT → SCROLL TO COMMENT
  // ==========================
  useEffect(() => {
    function handleHighlightClick(e) {
      if (!e.target.classList.contains("highlight")) return;

      const id = e.target.getAttribute("data-id");
      const commentElement = document.getElementById(`comment-${id}`);

      if (commentElement) {
        commentElement.scrollIntoView({ behavior: "smooth", block: "center" });
        commentElement.classList.add("active-comment");

        setTimeout(() => {
          commentElement.classList.remove("active-comment");
        }, 1500);
      }
    }

    document.addEventListener("click", handleHighlightClick);
    return () => {
      document.removeEventListener("click", handleHighlightClick);
    };
  }, []);

  // ==========================
  // CLICK COMMENT → SCROLL TO TEXT
  // ==========================
  function scrollToHighlight(id) {
    const highlightElement = document.getElementById(`highlight-${id}`);

    if (highlightElement) {
      highlightElement.scrollIntoView({ behavior: "smooth", block: "center" });
      highlightElement.classList.add("active-highlight");

      setTimeout(() => {
        highlightElement.classList.remove("active-highlight");
      }, 1500);
    }
  }

  // ==========================
  // UI
  // ==========================
  return (
    <div className="edit-container">
      <div className="edit-header">
        <button
          className="back-btn"
          onClick={() => navigate("/documents")}
        >
          ← Back
        </button>

        <div style={{ textAlign: "center" }}>
          <h2>Document Editor</h2>
          {latestVersion && (
            <small>
              Version {latestVersion.versionNumber} •{" "}
              {new Date(latestVersion.uploadedAt).toLocaleString()}
            </small>
          )}
        </div>

        <button
          className="view-btn"
          onClick={() => setViewMode((prev) => !prev)}
        >
          {viewMode ? "Switch to View" : "Switch to Edit"}
        </button>
      </div>

      <div className="edit-body">
        <div className="editor-layout">
          <div className="editor-panel">
            {loading ? (
              <p>Loading...</p>
            ) : viewMode ? (
              <div
                className="document-view"
                onMouseUp={handleTextSelection}
                dangerouslySetInnerHTML={{ __html: highlightedContent }}
              />
            ) : (
              <textarea
                value={content}
                onChange={(e) => setContent(e.target.value)}
              />
            )}
          </div>

          <div className="comments-panel">
            <h4>💬 Comments</h4>

            {comments.map((c) => (
              <div
                key={c.commentId}
                id={`comment-${c.commentId}`}
                className="comment-box"
                onClick={() => scrollToHighlight(c.commentId)}
              >
                <strong>{c.author?.name || "User"}:</strong>
                <div>{c.commentText}</div>
              </div>
            ))}

            {viewMode && selectionRange && (
              <>
                <textarea
                  value={newComment}
                  onChange={(e) => setNewComment(e.target.value)}
                  placeholder="Add comment..."
                />
                <button
                  className="add-comment-btn"
                  onClick={addComment}
                >
                  Add Comment
                </button>
              </>
            )}
          </div>
        </div>
      </div>

      <div className="edit-footer">
        <button
          className="save-btn"
          onClick={saveChanges}
          disabled={saving}
        >
          {saving ? "Saving..." : "Save Changes (New Version)"}
        </button>
      </div>
    </div>
  );
}