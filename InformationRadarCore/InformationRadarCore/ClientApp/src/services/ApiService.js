
import authService from '../components/api-authorization/AuthorizeService';

class ApiService {
    constructor() {
        this.adminCache = null;
    }

    async isAdmin() {
        if (this.adminCache !== null) {
            return this.adminCache;
        }

        const token = await authService.getAccessToken();
        const body = await fetch("/api/User", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        }).then(e => e.json());

        this.adminCache = body.isAdmin;
        return this.adminCache;
    }

    async bearer() {
        const token = await authService.getAccessToken();
        return { "Authorization": `Bearer ${token}` };
    }

    async basicGetRequest(apiRoute, options = {}) {
        const params = new URLSearchParams(options);

        const token = await authService.getAccessToken();
        return await fetch(apiRoute + "?" + params.toString(), {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        }).then(e => e.json());
    }

    getLighthouses(options = {}) {
        return this.basicGetRequest("/api/Lighthouse", options);
    }

    getTemplates(options = {}) {
        return this.basicGetRequest("/api/Template", options);
    }

    getReports(options = {}) {
        return this.basicGetRequest("/api/ReportingEngine/Reports", options);
    }

    getSites(options = {}) {
        return this.basicGetRequest("/api/SearchEngine/Sites", options);
    }

    getQueries(options = {}) {
        return this.basicGetRequest("/api/SearchEngine/Queries", options);
    }

    async getLighthouse(id) {
        return await fetch("/api/Lighthouse/" + id, { 
            headers: await this.bearer() 
        }).then(e => e.json());
    }

    async postData(apiEndpoint, data) {
        const token = await authService.getAccessToken();
        const response = await fetch(apiEndpoint, {
            method: "POST",
            credentials: "same-origin",
            headers: {
              "Content-Type": "application/json",
              "Authorization": `Bearer ${token}`,
            },
            body: JSON.stringify(data),
        });

        const body = await response.json();
        if (response.status === 200) {
            return body;
        }

        return Promise.reject(normalizeErrors(body));
    }

    generateReport(lighthouse, type = null, pageSize = null, pages = null) {
        const payload = {};
        if (pageSize) {
            payload.pageSize = pageSize;
        }
        if (pages) {
            payload.pages = pages;
        }
        if (type) {
            payload.type = type;
        }
        return this.postData("api/ReportingEngine/Generate/" + lighthouse, payload);
    }

    uploadTemplate(template) {
        return this.postData("/api/Template", template);
    }

    uploadCustomLighthouse(lighthouse) {
        return this.postData("/api/Lighthouse/NewCustom", lighthouse);
    }

    uploadTemplateLighthouse(lighthouse) {
        return this.postData("/api/Lighthouse/New", lighthouse);
    }

    runMessenger(lighthouse) {
        return this.postData(`/api/Lighthouse/${lighthouse}/SendMessages`);
    }

    runVisitor(lighthouse) {
        return this.postData(`/api/Lighthouse/${lighthouse}/Run`);
    }

    async downloadLogs(lighthouse) {
        const token = await authService.getAccessToken();
        return fetch("/api/ReportingEngine/ErrorLogs/" + lighthouse, {
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            })
            .then(res => res.blob())
            .then(blob => {
                var a = document.createElement("a");
                a.href = window.URL.createObjectURL(blob);
                a.setAttribute("download", "logs.zip");
                a.click();
            });
    }

    async enableLighthouse(lighthouse, enabled) {
        const token = await authService.getAccessToken();
        return fetch(`/api/Lighthouse/${lighthouse}`, {
            method: "PATCH",
            credentials: "same-origin",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`,
            },
            body: JSON.stringify({
                enabled
            })
        })
    }

    async subscribe(lighthouse, subscribed) {
        const token = await authService.getAccessToken();
        return fetch(`/api/Lighthouse/${lighthouse}/Subscription`, {
            method: "PUT",
            credentials: "same-origin",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`,
            },
            body: JSON.stringify({
                subscribed
            })
        });
    }

    async resolve(lighthouse) {
        const token = await authService.getAccessToken();
        return fetch(`/api/Lighthouse/${lighthouse}/Resolve`, {
            method: "PATCH",
            credentials: "same-origin",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`,
            }
        });
    }
}

// Adds message attribute to JSON deserialization errors
function normalizeErrors(body) {
    if (body.errors && Array.isArray(body.errors)) {
        body.message = body.errors.join("\n");
    } else if (typeof body.errors === "object") {
        body.message = Object.values(body.errors)
            .map(e => Array.isArray(e) ? e.join(", ") : e)
            .join("\n");
    } else if (typeof body.errors === "string") {
        body.message = body.errors;
    }
    return body.message;
}

const apiService = new ApiService();

export default apiService;