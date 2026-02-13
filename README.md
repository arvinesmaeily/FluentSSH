<p align="center">
  <img src="" width="80" alt="Fluent SSH Icon">
</p>
<h1 align="center">Fluent SSH</h1>

<div align="center">

  ![License](https://img.shields.io/github/license/arvinesmaeily/FluentSSH)
  ![Last Commit](https://img.shields.io/github/last-commit/arvinesmaeily/FluentSSH)
  ![Open Issues](https://img.shields.io/github/issues/arvinesmaeily/FluentSSH)

</div>
<br/>

A modern Windows application for creating a secure SOCKS5 proxy tunnel over SSH. Built with **WinUI 3** and the **Windows App SDK**, Fluent SSH is the next‑generation successor to [SSH‑Direct‑Client](https://github.com/arvinesmaeily/SSHDirectClient), redesigned from the ground up with a native Fluent Design experience.

This project utilizes the [SSH.NET](https://github.com/sshnet/SSH.NET) library.

## ✨ Features

* **Fluent Design UI** — Native WinUI 3 interface with Mica backdrop, NavigationView, and WinUI controls for a first‑class Windows 11 look and feel.
* **Profile Management** — Save and manage multiple SSH server configurations backed by a local SQLite database.
* **One‑Click Connect / Disconnect** — Dedicated green *Connect* and red *Disconnect* buttons with a real‑time status indicator.
* **Customizable SOCKS5 Proxy** — Configure the local proxy address and port directly from the Settings page.
* **Advanced SSH Options** — Adjust connection timeout, keep‑alive interval, and maximum retry count.
* **Light, Dark & System Themes** — Switch themes from Settings or follow the system default.
* **Connection Logs** — Expandable log panel with monospaced output for easy troubleshooting.

---

## 🚀 Getting Started

### Prerequisites

| Requirement | Details |
|---|---|
| **OS** | Windows 10 version 1809 (build 17763) or newer / Windows 11 |
| **Runtime** | .NET 10 Desktop Runtime |
| **Display** | 1280 × 720 or higher recommended |

### Installation

1. Go to the [**Releases**](https://github.com/arvinesmaeily/FluentSSH/releases) page.
2. Download & Install the latest release for your architecture (x64, x86, or ARM64).

### Building from Source

```bash
git clone https://github.com/arvinesmaeily/FluentSSH.git
cd FluentSSH
dotnet build FluentSSH/FluentSSH/FluentSSH.csproj
```

---

## 🔧 Usage Guide

The app uses a sidebar navigation with three sections: **Home**, **Configurations**, and **Settings**.

### 1. Configurations

Before connecting, add at least one server profile.

1. Navigate to the **Configurations** page from the sidebar.
2. Click **Add configuration**.
3. Fill in the dialog:
   * **Name** — A friendly label (e.g., "Work Server").
   * **Host Address** — IP or domain of the SSH server.
   * **Host Port** — SSH port (default `22`).
   * **Username & Password** — Your SSH credentials.
4. Click **Save**. The profile appears in the list where you can **Edit** or **Remove** it at any time.

### 2. Home — Connect & Disconnect

1. Go to the **Home** page.
2. Select a saved configuration.
3. Click **Connect**. The status indicator shows the current state:
   * 🔴 **Disconnected**
   * 🟡 **Connecting…** (with a progress ring)
   * 🟢 **Connected** — the SOCKS5 proxy address and port are displayed.
4. Click **Disconnect** to tear down the tunnel.

### 3. Settings

Accessible via the ⚙️ icon at the bottom of the navigation pane.

| Setting | Description | Default |
|---|---|---|
| **Proxy address** | Local IP for the SOCKS5 listener | `127.0.0.1` |
| **Proxy port** | Local port for the SOCKS5 listener | `1080` |
| **Connection timeout** | Seconds before a connection attempt times out (0 = 1 day) | `0` |
| **Keep‑alive interval** | Seconds between keep‑alive packets (0 = 1 minute) | `0` |
| **Maximum retries** | Reconnection attempts on failure | `10` |
| **App theme** | Light / Dark / Use system setting | System |

### 4. Logs

Expand the **Logs** section on the Home page to view real‑time connection output. Use the **Clear Logs** button to reset the panel.

---

## 🐞 Reporting Issues

If you encounter a bug or have a suggestion, please [open an issue](https://github.com/arvinesmaeily/FluentSSH/issues).

When reporting a connection problem, **include the relevant output from the Logs panel** to help diagnose the issue faster.

---

## 📜 License

This project is licensed under the [MIT License](LICENSE.txt).