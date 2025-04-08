import axios, { AxiosError, AxiosResponse } from 'axios'
import { toast } from 'react-toastify'
import { router } from '../router/Routes'

const sleep = () => new Promise((resolve) => setTimeout(resolve, 500))

axios.defaults.baseURL = 'https://localhost:7179/api'
axios.defaults.withCredentials = true

const responseBody = (response: AxiosResponse) => response.data

// Request interceptor: Add Authorization header
axios.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('authToken') // Retrieve the token stored in localStorage
        if (token) {
            config.headers['Authorization'] = `Bearer ${token}` // Add the token to the request header
        }
        return config
    },
    (error) => {
        return Promise.reject(error)
    }
)

axios.interceptors.response.use(
    async (response) => {
        await sleep()
        return response
    },
    (error: AxiosError) => {
        const { data, status } = error.response as AxiosResponse
        switch (status) {
            case 400:
                toast.error(data.title)
                break
            case 401:
                toast.error(data.message)
                break
            case 404:
                router.navigate('/server-error', { state: { error: data } })
                break
            case 500:
                router.navigate('/server-error')
                break
            default:
                break
        }
        return Promise.reject(error.response)
    }
)

const request = {
    get: (url: string) => axios.get(url).then(responseBody),
    getByParam: (url: string, params: string) => axios.get(url, { params }).then(responseBody),
    post: (url: string, body: object) => axios.post(url, body).then(responseBody),
    put: (url: string, body: object) => axios.put(url, body).then(responseBody),
    delete: (url: string) => axios.delete(url).then(responseBody),
    deleteByGroup: (url: string, body: object) => axios.delete(url, { data: body }).then(responseBody),
}

const Company = {
    list: () => request.get('Company'),
    details: (id: number) => request.get(`Company/${id}`),
}

const Create = {
    getGroup: (subscriptionid: string) => request.get(`Subscription/${subscriptionid}`),
    sendAllBvtJson: (body: object) => request.post(`/Creation/CreateBVTCache`, body),
    sendOneBvtJson: (body: object) => request.post(`/Creation/CreateBVTCacheByCase`, body),
    sendManJson: (body: object) => request.post(`/Creation/CreateManualCache`, body),
    sendPerfJson: (body: object) => request.post(`/Creation/CreatePerfCache`, body),
    sendAltJson: (body: object) => request.post(`/Creation/CreateAltCache`, body),
    sendBenchmarkRunJson: (body: object) => request.post('/BenchmarkRun/enqueue', body),
    // getBenchmarkRunJson: () => request.get('/Parameters')
}

const Delete = {
    getResource: (subscriptionid: string, group: string) => request.get(`ResourceDeletion/${subscriptionid}/${group}`),
    //sendDelGroupJson:(body: object)=>request.deleteByGroup(`/StackExchange/DeleteGroup`,body),
    sendDelGroupJson: (subscription: string, group: string) =>
        request.delete(`/AzureClient/DeleteResource?subscription=${subscription}&resourceGroupName=${group}`),
    sendDelGroupJsonT: (body: object) => request.deleteByGroup(`/ResourceDeletion/DeleteResource`, body),
}

const Other = {
    sendInsertJson: (body: object) => request.post(`/StackExchange/AddDataToRedis`, body),
    sendMedianJson: (body: object) => axios.post('/Median/sendMedianJson', body, { responseType: 'blob' }),
}

const Auth = {
    login: (username: string, password: string) => request.post(`/Auth/login`, { username, password }),
    register: (username: string, password: string) => request.post(`/Auth/register`, { username, password }),
    changePassword: (username: string, oldPassword: string, newPassword: string) =>
        request.post(`/Auth/change-password`, { username, oldPassword, newPassword }),
}

const TestErrors = {
    get400Error: () => request.get('buggy/bad-request'),
    get401Error: () => request.get('buggy/unauthorized'),
    get404Error: () => request.get('buggy/not-found'),
    get500Error: () => request.get('buggy/server-error'),
    getValidationError: () => request.get('buggy/validation-error'),
}

const SignalR = {
    hubUrl: '/createHub',
}

const agent = {
    Company,
    Create,
    Delete,
    Other,
    Auth,
    TestErrors,
    SignalR,
}

export default agent
