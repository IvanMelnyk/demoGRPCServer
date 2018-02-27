--
-- PostgreSQL database dump
--

-- Dumped from database version 10.2 (Ubuntu 10.2-1.pgdg16.04+1)
-- Dumped by pg_dump version 10.2 (Ubuntu 10.2-1.pgdg16.04+1)

-- Started on 2018-02-27 07:02:55 EET

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 1 (class 3079 OID 12998)
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- TOC entry 3005 (class 0 OID 0)
-- Dependencies: 1
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 202 (class 1259 OID 24732)
-- Name: AspNetRoleClaims; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "AspNetRoleClaims" (
    "Id" integer NOT NULL,
    "ClaimType" text,
    "ClaimValue" text,
    "RoleId" bigint NOT NULL
);


ALTER TABLE "AspNetRoleClaims" OWNER TO postgres;

--
-- TOC entry 201 (class 1259 OID 24730)
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE "AspNetRoleClaims_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE "AspNetRoleClaims_Id_seq" OWNER TO postgres;

--
-- TOC entry 3006 (class 0 OID 0)
-- Dependencies: 201
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE "AspNetRoleClaims_Id_seq" OWNED BY "AspNetRoleClaims"."Id";


--
-- TOC entry 204 (class 1259 OID 24748)
-- Name: AspNetUserClaims; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "AspNetUserClaims" (
    id integer NOT NULL,
    claim_type text,
    claim_value text,
    "Discriminator" text NOT NULL,
    user_id bigint NOT NULL
);


ALTER TABLE "AspNetUserClaims" OWNER TO postgres;

--
-- TOC entry 203 (class 1259 OID 24746)
-- Name: AspNetUserClaims_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE "AspNetUserClaims_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE "AspNetUserClaims_id_seq" OWNER TO postgres;

--
-- TOC entry 3007 (class 0 OID 0)
-- Dependencies: 203
-- Name: AspNetUserClaims_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE "AspNetUserClaims_id_seq" OWNED BY "AspNetUserClaims".id;


--
-- TOC entry 205 (class 1259 OID 24762)
-- Name: AspNetUserLogins; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "AspNetUserLogins" (
    login_provider text NOT NULL,
    provider_key text NOT NULL,
    "Discriminator" text NOT NULL,
    "ProviderDisplayName" text,
    user_id bigint NOT NULL
);


ALTER TABLE "AspNetUserLogins" OWNER TO postgres;

--
-- TOC entry 206 (class 1259 OID 24775)
-- Name: AspNetUserRoles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "AspNetUserRoles" (
    user_id bigint NOT NULL,
    role_id bigint NOT NULL,
    "Discriminator" text NOT NULL
);


ALTER TABLE "AspNetUserRoles" OWNER TO postgres;

--
-- TOC entry 207 (class 1259 OID 24793)
-- Name: AspNetUserTokens; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "AspNetUserTokens" (
    user_id bigint NOT NULL,
    login_provider text NOT NULL,
    name text NOT NULL,
    "Discriminator" text NOT NULL,
    value text
);


ALTER TABLE "AspNetUserTokens" OWNER TO postgres;

--
-- TOC entry 196 (class 1259 OID 24703)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE "__EFMigrationsHistory" OWNER TO postgres;

--
-- TOC entry 209 (class 1259 OID 33055)
-- Name: demo_records; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE demo_records (
    record_id bigint NOT NULL,
    stage boolean,
    creation_time timestamp without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    title character varying(100),
    data bytea,
    lang_specs integer DEFAULT 0 NOT NULL,
    approved_data bytea
);


ALTER TABLE demo_records OWNER TO postgres;

--
-- TOC entry 208 (class 1259 OID 33053)
-- Name: demo_records_record_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE demo_records_record_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE demo_records_record_id_seq OWNER TO postgres;

--
-- TOC entry 3008 (class 0 OID 0)
-- Dependencies: 208
-- Name: demo_records_record_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE demo_records_record_id_seq OWNED BY demo_records.record_id;


--
-- TOC entry 211 (class 1259 OID 33069)
-- Name: interims; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE interims (
    interim_id bigint NOT NULL,
    record_id bigint NOT NULL,
    created_by bigint DEFAULT 0 NOT NULL,
    creation_time timestamp without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    changed_data bytea,
    changed_data_hash uuid
);


ALTER TABLE interims OWNER TO postgres;

--
-- TOC entry 210 (class 1259 OID 33067)
-- Name: interims_interim_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE interims_interim_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE interims_interim_id_seq OWNER TO postgres;

--
-- TOC entry 3009 (class 0 OID 0)
-- Dependencies: 210
-- Name: interims_interim_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE interims_interim_id_seq OWNED BY interims.interim_id;


--
-- TOC entry 213 (class 1259 OID 33090)
-- Name: locks; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE locks (
    lock_id bigint NOT NULL,
    record_id bigint NOT NULL,
    locked_by bigint DEFAULT 0 NOT NULL,
    lock_time timestamp without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL
);


ALTER TABLE locks OWNER TO postgres;

--
-- TOC entry 212 (class 1259 OID 33088)
-- Name: locks_lock_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE locks_lock_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE locks_lock_id_seq OWNER TO postgres;

--
-- TOC entry 3010 (class 0 OID 0)
-- Dependencies: 212
-- Name: locks_lock_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE locks_lock_id_seq OWNED BY locks.lock_id;


--
-- TOC entry 198 (class 1259 OID 24710)
-- Name: roles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE roles (
    id bigint NOT NULL,
    "ConcurrencyStamp" text,
    name character varying,
    "NormalizedName" character varying(256)
);


ALTER TABLE roles OWNER TO postgres;

--
-- TOC entry 197 (class 1259 OID 24708)
-- Name: roles_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE roles_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE roles_id_seq OWNER TO postgres;

--
-- TOC entry 3011 (class 0 OID 0)
-- Dependencies: 197
-- Name: roles_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE roles_id_seq OWNED BY roles.id;


--
-- TOC entry 200 (class 1259 OID 24721)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE users (
    id bigint NOT NULL,
    access_failed_count integer NOT NULL,
    "ConcurrencyStamp" text,
    email character varying,
    email_confirmed boolean NOT NULL,
    lockout_enabled boolean NOT NULL,
    "LockoutEnd" timestamp with time zone,
    "NormalizedEmail" character varying(256),
    "NormalizedUserName" character varying(256),
    password_hash character varying,
    phone_number text,
    phone_number_confirmed boolean NOT NULL,
    security_stamp text,
    two_factor_enabled boolean NOT NULL,
    user_name character varying,
    expire_time timestamp with time zone NOT NULL,
    first_name character varying,
    join_date timestamp with time zone NOT NULL,
    last_name character varying
);


ALTER TABLE users OWNER TO postgres;

--
-- TOC entry 199 (class 1259 OID 24719)
-- Name: users_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE users_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE users_id_seq OWNER TO postgres;

--
-- TOC entry 3012 (class 0 OID 0)
-- Dependencies: 199
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE users_id_seq OWNED BY users.id;


--
-- TOC entry 2806 (class 2604 OID 24735)
-- Name: AspNetRoleClaims Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "AspNetRoleClaims" ALTER COLUMN "Id" SET DEFAULT nextval('"AspNetRoleClaims_Id_seq"'::regclass);


--
-- TOC entry 2807 (class 2604 OID 24751)
-- Name: AspNetUserClaims id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "AspNetUserClaims" ALTER COLUMN id SET DEFAULT nextval('"AspNetUserClaims_id_seq"'::regclass);


--
-- TOC entry 2808 (class 2604 OID 33058)
-- Name: demo_records record_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY demo_records ALTER COLUMN record_id SET DEFAULT nextval('demo_records_record_id_seq'::regclass);


--
-- TOC entry 2811 (class 2604 OID 33072)
-- Name: interims interim_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY interims ALTER COLUMN interim_id SET DEFAULT nextval('interims_interim_id_seq'::regclass);


--
-- TOC entry 2814 (class 2604 OID 33093)
-- Name: locks lock_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY locks ALTER COLUMN lock_id SET DEFAULT nextval('locks_lock_id_seq'::regclass);


--
-- TOC entry 2804 (class 2604 OID 24713)
-- Name: roles id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY roles ALTER COLUMN id SET DEFAULT nextval('roles_id_seq'::regclass);


--
-- TOC entry 2805 (class 2604 OID 24724)
-- Name: users id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY users ALTER COLUMN id SET DEFAULT nextval('users_id_seq'::regclass);


--
-- TOC entry 2987 (class 0 OID 24732)
-- Dependencies: 202
-- Data for Name: AspNetRoleClaims; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY "AspNetRoleClaims" ("Id", "ClaimType", "ClaimValue", "RoleId") FROM stdin;
\.


--
-- TOC entry 2989 (class 0 OID 24748)
-- Dependencies: 204
-- Data for Name: AspNetUserClaims; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY "AspNetUserClaims" (id, claim_type, claim_value, "Discriminator", user_id) FROM stdin;
\.


--
-- TOC entry 2990 (class 0 OID 24762)
-- Dependencies: 205
-- Data for Name: AspNetUserLogins; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY "AspNetUserLogins" (login_provider, provider_key, "Discriminator", "ProviderDisplayName", user_id) FROM stdin;
\.


--
-- TOC entry 2991 (class 0 OID 24775)
-- Dependencies: 206
-- Data for Name: AspNetUserRoles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY "AspNetUserRoles" (user_id, role_id, "Discriminator") FROM stdin;
\.


--
-- TOC entry 2992 (class 0 OID 24793)
-- Dependencies: 207
-- Data for Name: AspNetUserTokens; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY "AspNetUserTokens" (user_id, login_provider, name, "Discriminator", value) FROM stdin;
1	[AspNetUserStore]	AuthenticatorKey	CustomUserToken	R3CJVI5TYEL5AVGVBTSUUOQV6DYS7NM3
\.


--
-- TOC entry 2981 (class 0 OID 24703)
-- Dependencies: 196
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY "__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20180220134739_CustomScheme	2.0.1-rtm-125
\.


--
-- TOC entry 2994 (class 0 OID 33055)
-- Dependencies: 209
-- Data for Name: demo_records; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY demo_records (record_id, stage, creation_time, title, data, lang_specs, approved_data) FROM stdin;
\.


--
-- TOC entry 2996 (class 0 OID 33069)
-- Dependencies: 211
-- Data for Name: interims; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY interims (interim_id, record_id, created_by, creation_time, changed_data, changed_data_hash) FROM stdin;
\.


--
-- TOC entry 2998 (class 0 OID 33090)
-- Dependencies: 213
-- Data for Name: locks; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY locks (lock_id, record_id, locked_by, lock_time) FROM stdin;
\.


--
-- TOC entry 2983 (class 0 OID 24710)
-- Dependencies: 198
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY roles (id, "ConcurrencyStamp", name, "NormalizedName") FROM stdin;
\.


--
-- TOC entry 2985 (class 0 OID 24721)
-- Dependencies: 200
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY users (id, access_failed_count, "ConcurrencyStamp", email, email_confirmed, lockout_enabled, "LockoutEnd", "NormalizedEmail", "NormalizedUserName", password_hash, phone_number, phone_number_confirmed, security_stamp, two_factor_enabled, user_name, expire_time, first_name, join_date, last_name) FROM stdin;
2	0	7ee62a54-9c1e-476b-ba3b-26cd060ce684	mazepa_4@yahoo.com	f	t	\N	MAZEPA_4@YAHOO.COM	MAZEPA_4@YAHOO.COM	AQAAAAEAACcQAAAAEECjNAdQ2i8ISsUhywrfY3RPiTUZ+Fpe0JdWt8gyEnfFHUGdcAPRrDbrnasP1DIBsg==	\N	f	efcb05a6-6d5d-4ebc-ac67-ec6d6403e0a6	f	mazepa_4@yahoo.com	0001-01-01 02:02:04+02:02:04	Ivan	0001-01-01 02:02:04+02:02:04	Melnyk
1	0	88321353-487a-4868-8efc-08fa57429a74	my@mail.com	f	t	\N	MY@MAIL.COM	MY@MAIL.COM	AQAAAAEAACcQAAAAEL38Bqgf+nXIkma3zmgQrgBvkiRBknZLxHJFa7d9SYJ1m2w6ZW6HW5+M06auIQjDLw==	\N	f	15f364a0-1ce7-4b6e-8846-e8e0802ff298	f	my@mail.com	0001-01-01 02:02:04+02:02:04	Ivan	0001-01-01 02:02:04+02:02:04	Mazepa
\.


--
-- TOC entry 3013 (class 0 OID 0)
-- Dependencies: 201
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"AspNetRoleClaims_Id_seq"', 1, false);


--
-- TOC entry 3014 (class 0 OID 0)
-- Dependencies: 203
-- Name: AspNetUserClaims_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"AspNetUserClaims_id_seq"', 1, false);


--
-- TOC entry 3015 (class 0 OID 0)
-- Dependencies: 208
-- Name: demo_records_record_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('demo_records_record_id_seq', 1, false);


--
-- TOC entry 3016 (class 0 OID 0)
-- Dependencies: 210
-- Name: interims_interim_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('interims_interim_id_seq', 1, false);


--
-- TOC entry 3017 (class 0 OID 0)
-- Dependencies: 212
-- Name: locks_lock_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('locks_lock_id_seq', 1, false);


--
-- TOC entry 3018 (class 0 OID 0)
-- Dependencies: 197
-- Name: roles_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('roles_id_seq', 9, true);


--
-- TOC entry 3019 (class 0 OID 0)
-- Dependencies: 199
-- Name: users_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('users_id_seq', 2, true);


--
-- TOC entry 2828 (class 2606 OID 24740)
-- Name: AspNetRoleClaims PK_AspNetRoleClaims; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "AspNetRoleClaims"
    ADD CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id");


--
-- TOC entry 2831 (class 2606 OID 24756)
-- Name: AspNetUserClaims PK_AspNetUserClaims; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "AspNetUserClaims"
    ADD CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY (id);


--
-- TOC entry 2834 (class 2606 OID 24769)
-- Name: AspNetUserLogins PK_AspNetUserLogins; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "AspNetUserLogins"
    ADD CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY (login_provider, provider_key);


--
-- TOC entry 2837 (class 2606 OID 24782)
-- Name: AspNetUserRoles PK_AspNetUserRoles; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "AspNetUserRoles"
    ADD CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY (user_id, role_id);


--
-- TOC entry 2839 (class 2606 OID 24800)
-- Name: AspNetUserTokens PK_AspNetUserTokens; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "AspNetUserTokens"
    ADD CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY (user_id, login_provider, name);


--
-- TOC entry 2818 (class 2606 OID 24707)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- TOC entry 2820 (class 2606 OID 24718)
-- Name: roles PK_roles; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY roles
    ADD CONSTRAINT "PK_roles" PRIMARY KEY (id);


--
-- TOC entry 2824 (class 2606 OID 24729)
-- Name: users PK_users; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY users
    ADD CONSTRAINT "PK_users" PRIMARY KEY (id);


--
-- TOC entry 2841 (class 2606 OID 33065)
-- Name: demo_records demo_records_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY demo_records
    ADD CONSTRAINT demo_records_pkey PRIMARY KEY (record_id);


--
-- TOC entry 2847 (class 2606 OID 33079)
-- Name: interims interims_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY interims
    ADD CONSTRAINT interims_pkey PRIMARY KEY (interim_id);


--
-- TOC entry 2851 (class 2606 OID 33097)
-- Name: locks locks_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY locks
    ADD CONSTRAINT locks_pkey PRIMARY KEY (lock_id);


--
-- TOC entry 2822 (class 1259 OID 24811)
-- Name: EmailIndex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "EmailIndex" ON users USING btree ("NormalizedEmail");


--
-- TOC entry 2826 (class 1259 OID 24806)
-- Name: IX_AspNetRoleClaims_RoleId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" USING btree ("RoleId");


--
-- TOC entry 2829 (class 1259 OID 24807)
-- Name: IX_AspNetUserClaims_user_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetUserClaims_user_id" ON "AspNetUserClaims" USING btree (user_id);


--
-- TOC entry 2832 (class 1259 OID 24808)
-- Name: IX_AspNetUserLogins_user_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetUserLogins_user_id" ON "AspNetUserLogins" USING btree (user_id);


--
-- TOC entry 2835 (class 1259 OID 24809)
-- Name: IX_AspNetUserRoles_role_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetUserRoles_role_id" ON "AspNetUserRoles" USING btree (role_id);


--
-- TOC entry 2821 (class 1259 OID 24810)
-- Name: RoleNameIndex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "RoleNameIndex" ON roles USING btree ("NormalizedName");


--
-- TOC entry 2825 (class 1259 OID 24812)
-- Name: UserNameIndex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "UserNameIndex" ON users USING btree ("NormalizedUserName");


--
-- TOC entry 2843 (class 1259 OID 33087)
-- Name: created_by_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX created_by_index ON interims USING btree (created_by);


--
-- TOC entry 2844 (class 1259 OID 33086)
-- Name: demo_record_interims_id_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX demo_record_interims_id_index ON interims USING btree (record_id);


--
-- TOC entry 2848 (class 1259 OID 33103)
-- Name: demo_record_locks_id_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX demo_record_locks_id_index ON locks USING btree (record_id);


--
-- TOC entry 2845 (class 1259 OID 33085)
-- Name: interims_changed_data_hash_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX interims_changed_data_hash_index ON interims USING btree (changed_data_hash);


--
-- TOC entry 2849 (class 1259 OID 33104)
-- Name: locked_by_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX locked_by_index ON locks USING btree (locked_by);


--
-- TOC entry 2842 (class 1259 OID 33066)
-- Name: stage_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX stage_index ON demo_records USING btree (stage);


--
-- TOC entry 2852 (class 2606 OID 24741)
-- Name: AspNetRoleClaims FK_AspNetRoleClaims_roles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "AspNetRoleClaims"
    ADD CONSTRAINT "FK_AspNetRoleClaims_roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES roles(id) ON DELETE CASCADE;


--
-- TOC entry 2853 (class 2606 OID 24757)
-- Name: AspNetUserClaims FK_AspNetUserClaims_users_user_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "AspNetUserClaims"
    ADD CONSTRAINT "FK_AspNetUserClaims_users_user_id" FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;


--
-- TOC entry 2854 (class 2606 OID 24770)
-- Name: AspNetUserLogins FK_AspNetUserLogins_users_user_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "AspNetUserLogins"
    ADD CONSTRAINT "FK_AspNetUserLogins_users_user_id" FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;


--
-- TOC entry 2855 (class 2606 OID 24783)
-- Name: AspNetUserRoles FK_AspNetUserRoles_roles_role_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_roles_role_id" FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE CASCADE;


--
-- TOC entry 2856 (class 2606 OID 24788)
-- Name: AspNetUserRoles FK_AspNetUserRoles_users_user_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_users_user_id" FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;


--
-- TOC entry 2857 (class 2606 OID 24801)
-- Name: AspNetUserTokens FK_AspNetUserTokens_users_user_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "AspNetUserTokens"
    ADD CONSTRAINT "FK_AspNetUserTokens_users_user_id" FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;


--
-- TOC entry 2858 (class 2606 OID 33080)
-- Name: interims interims_record_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY interims
    ADD CONSTRAINT interims_record_id_fkey FOREIGN KEY (record_id) REFERENCES demo_records(record_id) ON DELETE CASCADE;


--
-- TOC entry 2859 (class 2606 OID 33098)
-- Name: locks locks_record_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY locks
    ADD CONSTRAINT locks_record_id_fkey FOREIGN KEY (record_id) REFERENCES demo_records(record_id) ON DELETE CASCADE;


-- Completed on 2018-02-27 07:02:55 EET

--
-- PostgreSQL database dump complete
--


﻿-------------------------------------------------------------------------------------------------
--- demo_records
-------------------------------------------------------------------------------------------------
CREATE TABLE "public"."demo_records"(
	"record_id"			bigserial NOT NULL PRIMARY KEY,
	"stage"						boolean,
	"creation_time"				timestamp without time zone NOT NULL DEFAULT '0001-01-01 00:00:00',
	"title"						varchar(100),
	"data"						bytea,
	"lang_specs"					integer NOT NULL DEFAULT 0,
	"approved_data"				bytea
);
CREATE INDEX "stage_index" ON "public"."demo_records" ("stage");
-------------------------------------------------------------------------------------------------
---`
-------------------------------------------------------------------------------------------------

-------------------------------------------------------------------------------------------------
--- interims
-------------------------------------------------------------------------------------------------
CREATE TABLE "public"."interims"(
	"interim_id"				bigserial NOT NULL PRIMARY KEY,
	"record_id"			bigint NOT NULL REFERENCES "public"."demo_records" ("record_id") ON DELETE CASCADE,
	"created_by"				bigint NOT NULL DEFAULT 0,
	"creation_time"				timestamp without time zone NOT NULL DEFAULT '0001-01-01 00:00:00',
	"changed_data"				bytea,
	"changed_data_hash"			uuid
);

CREATE INDEX "interims_changed_data_hash_index" ON "public"."interims" ("changed_data_hash");
CREATE INDEX "demo_record_interims_id_index" ON "public"."interims" ("record_id");
CREATE INDEX "created_by_index" ON "public"."interims" ("created_by");
-------------------------------------------------------------------------------------------------
---
-------------------------------------------------------------------------------------------------

-------------------------------------------------------------------------------------------------
--- locks
-------------------------------------------------------------------------------------------------
CREATE TABLE "public"."locks"(
	"lock_id"					bigserial NOT NULL PRIMARY KEY,
	"record_id"			bigint NOT NULL  REFERENCES "public"."demo_records" ("record_id") ON DELETE CASCADE,
	"locked_by"					bigint NOT NULL DEFAULT 0,
	"lock_time"					timestamp without time zone NOT NULL DEFAULT '0001-01-01 00:00:00'
);

CREATE INDEX "demo_record_locks_id_index" ON "public"."locks" ("record_id");
CREATE UNIQUE INDEX "locked_by_index" ON "public"."locks" ("locked_by");
-------------------------------------------------------------------------------------------------
---
-------------------------------------------------------------------------------------------------
