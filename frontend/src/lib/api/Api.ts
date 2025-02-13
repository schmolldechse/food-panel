/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export interface Post {
	/** @format uuid */
	id?: string;
	/** @format uuid */
	creatorId?: string;
	creator?: User;
	/** @maxLength 128 */
	title?: string | null;
	message?: string | null;
	ratings?: Rating[] | null;
}

export interface PostOutDto {
	/** @format uuid */
	id?: string;
	/** @format uuid */
	creatorId?: string;
	creatorName?: string | null;
	title?: string | null;
	message?: string | null;
	/** @format int32 */
	commentAmount?: number;
	/** @format double */
	averageRating?: number;
}

export interface ProblemDetails {
	type?: string | null;
	title?: string | null;
	/** @format int32 */
	status?: number | null;
	detail?: string | null;
	instance?: string | null;
	[key: string]: any;
}

export interface Rating {
	/** @format uuid */
	id?: string;
	/** @format uuid */
	postId?: string;
	post?: Post;
	/** @format uuid */
	creatorId?: string;
	creator?: User;
	/** @format double */
	stars?: number;
	message?: string | null;
}

export interface RatingInDto {
	/** @format uuid */
	postId?: string;
	/** @format double */
	stars?: number;
	message?: string | null;
}

export interface User {
	/** @format uuid */
	id?: string;
	userName?: string | null;
	normalizedUserName?: string | null;
	email?: string | null;
	normalizedEmail?: string | null;
	emailConfirmed?: boolean;
	passwordHash?: string | null;
	securityStamp?: string | null;
	concurrencyStamp?: string | null;
	phoneNumber?: string | null;
	phoneNumberConfirmed?: boolean;
	twoFactorEnabled?: boolean;
	/** @format date-time */
	lockoutEnd?: string | null;
	lockoutEnabled?: boolean;
	/** @format int32 */
	accessFailedCount?: number;
	/** @maxLength 255 */
	name?: string | null;
	/** @maxLength 32 */
	userHandle?: string | null;
	posts?: Post[] | null;
	ratings?: Rating[] | null;
}

export interface UserOutDto {
	/** @format uuid */
	id?: string;
	name?: string | null;
	userHandle?: string | null;
	/** @format int32 */
	postCount?: number;
	/** @format int32 */
	ratingCount?: number;
	/** @format double */
	averageRating?: number;
}

import type { AxiosInstance, AxiosRequestConfig, AxiosResponse, HeadersDefaults, ResponseType } from "axios";
import axios from "axios";

export type QueryParamsType = Record<string | number, any>;

export interface FullRequestParams extends Omit<AxiosRequestConfig, "data" | "params" | "url" | "responseType"> {
	/** set parameter to `true` for call `securityWorker` for this request */
	secure?: boolean;
	/** request path */
	path: string;
	/** content type of request body */
	type?: ContentType;
	/** query params */
	query?: QueryParamsType;
	/** format of response (i.e. response.json() -> format: "json") */
	format?: ResponseType;
	/** request body */
	body?: unknown;
}

export type RequestParams = Omit<FullRequestParams, "body" | "method" | "query" | "path">;

export interface ApiConfig<SecurityDataType = unknown> extends Omit<AxiosRequestConfig, "data" | "cancelToken"> {
	securityWorker?: (securityData: SecurityDataType | null) => Promise<AxiosRequestConfig | void> | AxiosRequestConfig | void;
	secure?: boolean;
	format?: ResponseType;
}

export enum ContentType {
	Json = "application/json",
	FormData = "multipart/form-data",
	UrlEncoded = "application/x-www-form-urlencoded",
	Text = "text/plain"
}

export class HttpClient<SecurityDataType = unknown> {
	public instance: AxiosInstance;
	private securityData: SecurityDataType | null = null;
	private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
	private secure?: boolean;
	private format?: ResponseType;

	constructor({ securityWorker, secure, format, ...axiosConfig }: ApiConfig<SecurityDataType> = {}) {
		this.instance = axios.create({ ...axiosConfig, baseURL: axiosConfig.baseURL || "" });
		this.secure = secure;
		this.format = format;
		this.securityWorker = securityWorker;
	}

	public setSecurityData = (data: SecurityDataType | null) => {
		this.securityData = data;
	};

	protected mergeRequestParams(params1: AxiosRequestConfig, params2?: AxiosRequestConfig): AxiosRequestConfig {
		const method = params1.method || (params2 && params2.method);

		return {
			...this.instance.defaults,
			...params1,
			...(params2 || {}),
			headers: {
				...((method && this.instance.defaults.headers[method.toLowerCase() as keyof HeadersDefaults]) || {}),
				...(params1.headers || {}),
				...((params2 && params2.headers) || {})
			}
		};
	}

	protected stringifyFormItem(formItem: unknown) {
		if (typeof formItem === "object" && formItem !== null) {
			return JSON.stringify(formItem);
		} else {
			return `${formItem}`;
		}
	}

	protected createFormData(input: Record<string, unknown>): FormData {
		if (input instanceof FormData) {
			return input;
		}
		return Object.keys(input || {}).reduce((formData, key) => {
			const property = input[key];
			const propertyContent: any[] = property instanceof Array ? property : [property];

			for (const formItem of propertyContent) {
				const isFileType = formItem instanceof Blob || formItem instanceof File;
				formData.append(key, isFileType ? formItem : this.stringifyFormItem(formItem));
			}

			return formData;
		}, new FormData());
	}

	public request = async <T = any, _E = any>({
		secure,
		path,
		type,
		query,
		format,
		body,
		...params
	}: FullRequestParams): Promise<AxiosResponse<T>> => {
		const secureParams =
			((typeof secure === "boolean" ? secure : this.secure) &&
				this.securityWorker &&
				(await this.securityWorker(this.securityData))) ||
			{};
		const requestParams = this.mergeRequestParams(params, secureParams);
		const responseFormat = format || this.format || undefined;

		if (type === ContentType.FormData && body && body !== null && typeof body === "object") {
			body = this.createFormData(body as Record<string, unknown>);
		}

		if (type === ContentType.Text && body && body !== null && typeof body !== "string") {
			body = JSON.stringify(body);
		}

		return this.instance.request({
			...requestParams,
			headers: {
				...(requestParams.headers || {}),
				...(type ? { "Content-Type": type } : {})
			},
			params: query,
			responseType: responseFormat,
			data: body,
			url: path
		});
	};
}

/**
 * @title Foodpanel
 * @version v1
 *
 * API Description for Project
 */
export class Api<SecurityDataType extends unknown> extends HttpClient<SecurityDataType> {
	auth = {
		/**
		 * No description
		 *
		 * @tags Auth
		 * @name V1AuthDetail
		 * @request GET:/api/v1/Auth/@{handle}
		 */
		v1AuthDetail: (handle: string, params: RequestParams = {}) =>
			this.request<UserOutDto, ProblemDetails>({
				path: `/api/v1/Auth/@${handle}`,
				method: "GET",
				format: "json",
				...params
			}),

		/**
		 * No description
		 *
		 * @tags Auth
		 * @name V1AuthMeList
		 * @request GET:/api/v1/Auth/@me
		 */
		v1AuthMeList: (params: RequestParams = {}) =>
			this.request<UserOutDto, UserOutDto>({
				path: `/api/v1/Auth/@me`,
				method: "GET",
				format: "json",
				...params
			}),

		/**
		 * No description
		 *
		 * @tags Auth
		 * @name V1AuthSigninDetail
		 * @request GET:/api/v1/Auth/signin/{providerName}
		 */
		v1AuthSigninDetail: (
			providerName: string,
			query?: {
				returnUrl?: string;
			},
			params: RequestParams = {}
		) =>
			this.request<void, any>({
				path: `/api/v1/Auth/signin/${providerName}`,
				method: "GET",
				query: query,
				...params
			}),

		/**
		 * No description
		 *
		 * @tags Auth
		 * @name V1AuthSignoutList
		 * @request GET:/api/v1/Auth/signout
		 */
		v1AuthSignoutList: (
			query?: {
				returnUrl?: string;
			},
			params: RequestParams = {}
		) =>
			this.request<UserOutDto, UserOutDto>({
				path: `/api/v1/Auth/signout`,
				method: "GET",
				query: query,
				format: "json",
				...params
			}),

		/**
		 * No description
		 *
		 * @tags Auth
		 * @name V1AuthExternalCallbackList
		 * @request GET:/api/v1/Auth/external/callback
		 */
		v1AuthExternalCallbackList: (
			query?: {
				returnUrl?: string;
			},
			params: RequestParams = {}
		) =>
			this.request<void, void | ProblemDetails>({
				path: `/api/v1/Auth/external/callback`,
				method: "GET",
				query: query,
				...params
			})
	};
	post = {
		/**
		 * No description
		 *
		 * @tags Post
		 * @name V1PostList
		 * @request GET:/api/v1/Post
		 */
		v1PostList: (params: RequestParams = {}) =>
			this.request<PostOutDto[], any>({
				path: `/api/v1/Post`,
				method: "GET",
				format: "json",
				...params
			}),

		/**
		 * No description
		 *
		 * @tags Post
		 * @name V1PostCreate
		 * @request POST:/api/v1/Post
		 */
		v1PostCreate: (
			data: {
				Title?: string;
				Message?: string;
				/** @format binary */
				Image?: File;
			},
			params: RequestParams = {}
		) =>
			this.request<void, any>({
				path: `/api/v1/Post`,
				method: "POST",
				body: data,
				type: ContentType.FormData,
				...params
			}),

		/**
		 * No description
		 *
		 * @tags Post
		 * @name V1PostDelete
		 * @request DELETE:/api/v1/Post
		 */
		v1PostDelete: (
			data: {
				/** @format uuid */
				postId: string;
			},
			params: RequestParams = {}
		) =>
			this.request<void, ProblemDetails>({
				path: `/api/v1/Post`,
				method: "DELETE",
				body: data,
				type: ContentType.FormData,
				...params
			}),

		/**
		 * No description
		 *
		 * @tags Post
		 * @name V1PostUserDetail
		 * @request GET:/api/v1/Post/user/{userId}
		 */
		v1PostUserDetail: (userId: string, params: RequestParams = {}) =>
			this.request<PostOutDto[], ProblemDetails>({
				path: `/api/v1/Post/user/${userId}`,
				method: "GET",
				format: "json",
				...params
			}),

		/**
		 * No description
		 *
		 * @tags Post
		 * @name V1PostDetail
		 * @request GET:/api/v1/Post/{postId}
		 */
		v1PostDetail: (postId: string, params: RequestParams = {}) =>
			this.request<PostOutDto, ProblemDetails>({
				path: `/api/v1/Post/${postId}`,
				method: "GET",
				format: "json",
				...params
			})
	};
	ratings = {
		/**
		 * No description
		 *
		 * @tags Ratings
		 * @name V1RatingsCreate
		 * @request POST:/api/v1/Ratings
		 */
		v1RatingsCreate: (data: RatingInDto, params: RequestParams = {}) =>
			this.request<void, ProblemDetails>({
				path: `/api/v1/Ratings`,
				method: "POST",
				body: data,
				type: ContentType.Json,
				...params
			}),

		/**
		 * No description
		 *
		 * @tags Ratings
		 * @name V1RatingsDelete
		 * @request DELETE:/api/v1/Ratings
		 */
		v1RatingsDelete: (
			query: {
				/** @format uuid */
				postId: string;
			},
			params: RequestParams = {}
		) =>
			this.request<void, ProblemDetails>({
				path: `/api/v1/Ratings`,
				method: "DELETE",
				query: query,
				...params
			}),

		/**
		 * No description
		 *
		 * @tags Ratings
		 * @name V1RatingsGetRatingsByPostIdList
		 * @request GET:/api/v1/Ratings/getRatingsByPostId
		 */
		v1RatingsGetRatingsByPostIdList: (
			query: {
				/** @format uuid */
				postId: string;
			},
			params: RequestParams = {}
		) =>
			this.request<Rating[], ProblemDetails>({
				path: `/api/v1/Ratings/getRatingsByPostId`,
				method: "GET",
				query: query,
				format: "json",
				...params
			}),

		/**
		 * No description
		 *
		 * @tags Ratings
		 * @name V1RatingsGetAllRatingsList
		 * @request GET:/api/v1/Ratings/getAllRatings
		 */
		v1RatingsGetAllRatingsList: (params: RequestParams = {}) =>
			this.request<Rating[], any>({
				path: `/api/v1/Ratings/getAllRatings`,
				method: "GET",
				format: "json",
				...params
			})
	};
}
