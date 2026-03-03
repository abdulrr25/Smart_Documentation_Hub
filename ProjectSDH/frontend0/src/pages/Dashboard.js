import "../styles/Dashboard.css";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";

export default function Dashboard() {
  const navigate = useNavigate();
  const [userName, setUserName] = useState("");

  function logout() {
    localStorage.clear();
    window.location.href = "/";
  }

  // 🔹 Get user name from JWT
  useEffect(() => {
    const token = localStorage.getItem("token");

    if (token) {
      try {
        const decoded = jwtDecode(token);

        // If JWT contains name
        if (decoded.name) {
          // Show only first name
          setUserName(decoded.name.split(" ")[0]);
        } else if (decoded.email) {
          // Fallback to email username
          setUserName(decoded.email.split("@")[0]);
        }
      } catch (error) {
        console.error("Invalid token");
      }
    }
  }, []);

  return (
    <div className="dashboard-container">
      {/* Header */}
      <div className="dashboard-header">
        <div>
          <h1>Welcome, {userName || "User"} 👋</h1>
          <p>Manage your documents smartly & securely</p>
        </div>
        <button className="logout-btn" onClick={logout}>
          Logout
        </button>
      </div>

      {/* Cards */}
      <div className="dashboard-cards">
        <div
          className="dashboard-card upload-card"
          onClick={() => navigate("/upload")}
        >
          <h2>⬆ Upload Document</h2>
          <p>Add a new document from your device</p>
        </div>

        <div
          className="dashboard-card documents-card"
          onClick={() => navigate("/documents")}
        >
          <h2>📁 My Documents</h2>
          <p>View, edit and manage your files</p>
        </div>
      </div>

      {/* Info Section */}
      <div className="dashboard-info">
        <h2>Smart Document Hub</h2>
        <p>
          A centralized platform designed to securely upload, organize,
          preview, edit and manage all your important documents with version
          control and easy access.
        </p>
      </div>
    </div>
  );
}