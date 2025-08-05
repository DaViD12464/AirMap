// wwwroot/js/map/sessions.js

class SessionCache {
    static set(key, value) {
        try {
            const serializedValue = JSON.stringify(value);
            sessionStorage.setItem(key, serializedValue);
            return true;
        } catch (error) {
            console.error("Error setting session cache:", error);
            return false;
        }
    }

    static get(key, defaultValue = null) {
        try {
            const value = sessionStorage.getItem(key);
            return value ? JSON.parse(value) : defaultValue;
        } catch (error) {
            console.error("Error getting session cache:", error);
            return defaultValue;
        }
    }

    static remove(key) {
        sessionStorage.removeItem(key);
    }

    static clear() {
        sessionStorage.clear();
    }

    static exists(key) {
        return sessionStorage.getItem(key) !== null;
    }

    static getAllKeys() {
        return Object.keys(sessionStorage);
    }
}

export default SessionCache;