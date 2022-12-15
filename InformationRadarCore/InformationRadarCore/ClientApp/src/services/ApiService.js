
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
        return this.basicPaginatorRequest("api/Lighthouse", options);
    }

    async getLighthouse(id) {
        return await fetch("api/Lighthouse/" + id, { 
            headers: await this.bearer() 
        }).then(e => e.json());
    }

    updateLighthouse(options) {
        //return bearer().then(headers => fetch())
    }
}

const apiService = new ApiService();

export default apiService;