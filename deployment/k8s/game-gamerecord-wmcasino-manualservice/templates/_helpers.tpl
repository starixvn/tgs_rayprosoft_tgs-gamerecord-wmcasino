{{/*
Expand the name of the chart.
*/}}
{{- define "game-gamerecord-wmcasino-manualservice.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "game-gamerecord-wmcasino-manualservice.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "game-gamerecord-wmcasino-manualservice.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "game-gamerecord-wmcasino-manualservice.labels" -}}
helm.sh/chart: {{ include "game-gamerecord-wmcasino-manualservice.chart" . }}
{{ include "game-gamerecord-wmcasino-manualservice.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "game-gamerecord-wmcasino-manualservice.selectorLabels" -}}
app.kubernetes.io/name: {{ include "game-gamerecord-wmcasino-manualservice.name" . }}
app: {{ include "game-gamerecord-wmcasino-manualservice.name" . }}
tgs.kind: game
tgs.game.kind: gamerecord
tgs.gamerecord.kind: manualservice
tgs.gameprovider.kind: wmcasino
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "game-gamerecord-wmcasino-manualservice.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "game-gamerecord-wmcasino-manualservice.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}
