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

export interface PostOutDto {
	/** @format uuid */
	id?: string;
	/** @format uuid */
	creatorId?: string;
	title?: string | null;
	message?: string | null;
	/** @format int32 */
	commentAmount?: number;
	/** @format double */
	averageRating?: number;
}

export interface RatingInDto {
	/** @format uuid */
	postId?: string;
	/** @format uuid */
	userId?: string;
	message?: string | null;
}

export type QueryParamsType = Record<string | number, any>;
export type ResponseFormat = keyof Omit<Body, "body" | "bodyUsed">;

export interface FullRequestParams extends Omit<RequestInit, "body"> {
	/** set parameter to `true` for call `securityWorker` for this request */
	secure?: boolean;
	/** request path */
	path: string;
	/** content type of request body */
	type?: ContentType;
	/** query params */
	query?: QueryParamsType;
	/** format of response (i.e. response.json() -> format: "json") */
	format?: ResponseFormat;
	/** request body */
	body?: unknown;
	/** base url */
	baseUrl?: string;
	/** request cancellation token */
	cancelToken?: CancelToken;
}

export type RequestParams = Omit<FullRequestParams, "body" | "method" | "query" | "path">;

export interface ApiConfig<SecurityDataType = unknown> {
	baseUrl?: string;
	baseApiParams?: Omit<RequestParams, "baseUrl" | "cancelToken" | "signal">;
	securityWorker?: (securityData: SecurityDataType | null) => Promise<RequestParams | void> | RequestParams | void;
	customFetch?: typeof fetch;
}

export interface HttpResponse<D extends unknown, E extends unknown = unknown> extends Response {
	data: D;
	error: E;
}

type CancelToken = Symbol | string | number;

export enum ContentType {
	Json = "application/json",
	FormData = "multipart/form-data",
	UrlEncoded = "application/x-www-form-urlencoded",
	Text = "text/plain"
}

export class HttpClient<SecurityDataType = unknown> {
	public baseUrl: string = "";
	private securityData: SecurityDataType | null = null;
	private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
	private abortControllers = new Map<CancelToken, AbortController>();
	private customFetch = (...fetchParams: Parameters<typeof fetch>) => fetch(...fetchParams);

	private baseApiParams: RequestParams = {
		credentials: "same-origin",
		headers: {},
		redirect: "follow",
		referrerPolicy: "no-referrer"
	};

	constructor(apiConfig: ApiConfig<SecurityDataType> = {}) {
		Object.assign(this, apiConfig);
	}

	public setSecurityData = (data: SecurityDataType | null) => {
		this.securityData = data;
	};

	protected encodeQueryParam(key: string, value: any) {
		const encodedKey = encodeURIComponent(key);
		return `${encodedKey}=${encodeURIComponent(typeof value === "number" ? value : `${value}`)}`;
	}

	protected addQueryParam(query: QueryParamsType, key: string) {
		return this.encodeQueryParam(key, query[key]);
	}

	protected addArrayQueryParam(query: QueryParamsType, key: string) {
		const value = query[key];
		return value.map((v: any) => this.encodeQueryParam(key, v)).join("&");
	}

	protected toQueryString(rawQuery?: QueryParamsType): string {
		const query = rawQuery || {};
		const keys = Object.keys(query).filter((key) => "undefined" !== typeof query[key]);
		return keys
			.map((key) => (Array.isArray(query[key]) ? this.addArrayQueryParam(query, key) : this.addQueryParam(query, key)))
			.join("&");
	}

	protected addQueryParams(rawQuery?: QueryParamsType): string {
		const queryString = this.toQueryString(rawQuery);
		return queryString ? `?${queryString}` : "";
	}

	private contentFormatters: Record<ContentType, (input: any) => any> = {
		[ContentType.Json]: (input: any) =>
			input !== null && (typeof input === "object" || typeof input === "string") ? JSON.stringify(input) : input,
		[ContentType.Text]: (input: any) => (input !== null && typeof input !== "string" ? JSON.stringify(input) : input),
		[ContentType.FormData]: (input: any) =>
			Object.keys(input || {}).reduce((formData, key) => {
				const property = input[key];
				formData.append(
					key,
					property instanceof Blob
						? property
						: typeof property === "object" && property !== null
							? JSON.stringify(property)
							: `${property}`
				);
				return formData;
			}, new FormData()),
		[ContentType.UrlEncoded]: (input: any) => this.toQueryString(input)
	};

	protected mergeRequestParams(params1: RequestParams, params2?: RequestParams): RequestParams {
		return {
			...this.baseApiParams,
			...params1,
			...(params2 || {}),
			headers: {
				...(this.baseApiParams.headers || {}),
				...(params1.headers || {}),
				...((params2 && params2.headers) || {})
			}
		};
	}

	protected createAbortSignal = (cancelToken: CancelToken): AbortSignal | undefined => {
		if (this.abortControllers.has(cancelToken)) {
			const abortController = this.abortControllers.get(cancelToken);
			if (abortController) {
				return abortController.signal;
			}
			return void 0;
		}

		const abortController = new AbortController();
		this.abortControllers.set(cancelToken, abortController);
		return abortController.signal;
	};

	public abortRequest = (cancelToken: CancelToken) => {
		const abortController = this.abortControllers.get(cancelToken);

		if (abortController) {
			abortController.abort();
			this.abortControllers.delete(cancelToken);
		}
	};

	public request = async <T = any, E = any>({
		body,
		secure,
		path,
		type,
		query,
		format,
		baseUrl,
		cancelToken,
		...params
	}: FullRequestParams): Promise<HttpResponse<T, E>> => {
		const secureParams =
			((typeof secure === "boolean" ? secure : this.baseApiParams.secure) &&
				this.securityWorker &&
				(await this.securityWorker(this.securityData))) ||
			{};
		const requestParams = this.mergeRequestParams(params, secureParams);
		const queryString = query && this.toQueryString(query);
		const payloadFormatter = this.contentFormatters[type || ContentType.Json];
		const responseFormat = format || requestParams.format;

		return this.customFetch(`${baseUrl || this.baseUrl || ""}${path}${queryString ? `?${queryString}` : ""}`, {
			...requestParams,
			headers: {
				...(requestParams.headers || {}),
				...(type && type !== ContentType.FormData ? { "Content-Type": type } : {})
			},
			signal: (cancelToken ? this.createAbortSignal(cancelToken) : requestParams.signal) || null,
			body: typeof body === "undefined" || body === null ? null : payloadFormatter(body)
		}).then(async (response) => {
			const r = response.clone() as HttpResponse<T, E>;
			r.data = null as unknown as T;
			r.error = null as unknown as E;

			const data = !responseFormat
				? r
				: await response[responseFormat]()
						.then((data) => {
							if (r.ok) {
								r.data = data;
							} else {
								r.error = data;
							}
							return r;
						})
						.catch((e) => {
							r.error = e;
							return r;
						});

			if (cancelToken) {
				this.abortControllers.delete(cancelToken);
			}

			if (!response.ok) throw data;
			return data;
		});
	};
}

/**
 * @title Foodpanel
 * @version v1
 *
 * API Description for Project
 */
export class Api<SecurityDataType extends unknown> {
	http: HttpClient<SecurityDataType>;

	constructor(http: HttpClient<SecurityDataType>) {
		this.http = http;
	}

	post = {
		/**
		 * No description
		 *
		 * @tags Post
		 * @name V1PostList
		 * @request GET:/api/v1/Post
		 */
		v1PostList: (params: RequestParams = {}) =>
			this.http.request<PostOutDto[], any>({
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
				/** @format uuid */
				CreatorId?: string;
				Title?: string;
				Message?: string;
				/** @format binary */
				Image?: File;
			},
			params: RequestParams = {}
		) =>
			this.http.request<void, any>({
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
			this.http.request<void, any>({
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
		 * @name V1PostDetail
		 * @request GET:/api/v1/Post/{userId}
		 */
		v1PostDetail: (userId: string, params: RequestParams = {}) =>
			this.http.request<void, any>({
				path: `/api/v1/Post/${userId}`,
				method: "GET",
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
			this.http.request<void, any>({
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
			data: string,
			params: RequestParams = {}
		) =>
			this.http.request<void, any>({
				path: `/api/v1/Ratings`,
				method: "DELETE",
				query: query,
				body: data,
				type: ContentType.Json,
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
			this.http.request<void, any>({
				path: `/api/v1/Ratings/getRatingsByPostId`,
				method: "GET",
				query: query,
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
			this.http.request<void, any>({
				path: `/api/v1/Ratings/getAllRatings`,
				method: "GET",
				...params
			})
	};
	user = {
		/**
		 * No description
		 *
		 * @tags User
		 * @name V1UserDetail
		 * @request GET:/api/v1/User/{userId}
		 */
		v1UserDetail: (userId: string, params: RequestParams = {}) =>
			this.http.request<void, any>({
				path: `/api/v1/User/${userId}`,
				method: "GET",
				...params
			})
	};
}
