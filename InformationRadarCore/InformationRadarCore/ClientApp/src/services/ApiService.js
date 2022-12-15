
import authService from '../components/api-authorization/AuthorizeService';

class ApiService {
    constructor() {
        this.adminCache = null;
    }

    async isAdmin() {
        if (this.adminCache != null) {
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

    async basicPaginatorRequest(apiRoute, options = {}) {
        const params = new URLSearchParams();
        if (options.pageSize !== undefined) {
            params.set("pageSize", options.pageSize);
        }
        if (options.cursor !== undefined) {
            params.set("cursor", options.cursor);
        }

        const token = await authService.getAccessToken();
        return await fetch(apiRoute + "?" + params.toString(), {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        }).then(e => e.json());
    }

    getLighthouses(options = {}) {
        return this.basicPaginatorRequest("/api/Lighthouse", options);
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

    uploadTemplate(template) {
        return this.postData("/api/Template", template);
    }
}

// Adds message attribute to JSON deserialization errors
function normalizeErrors(body) {
    if (typeof body.errors === "object") {
        body.message = Object.values(body.errors)
            .map(e => e.join(", "))
            .join("; ");
    } else if (typeof body.errors === "string") {
        body.message = body.errors;
    }
    return body.message;
}

const apiService = new ApiService();

export default apiService;