pipeline {
    agent any

    environment {
        IMAGE_NAME = "onlinebookstore"
        CONTAINER_NAME = "onlinebookstore-app"
        PORT = "8080"
    }

    stages {

        stage('Debug Workspace') {
            steps {
                sh 'pwd'
                sh 'ls -la'
            }
        }

        stage('Check Docker') {
            steps {
                sh 'docker version'
                sh 'docker ps'
            }
        }

        stage('Build Docker Image') {
            steps {
                sh "docker build -t $IMAGE_NAME:latest ."
            }
        }

        stage('Stop Old Container') {
            steps {
                sh """
                    docker stop $CONTAINER_NAME || true
                    docker rm $CONTAINER_NAME || true
                """
            }
        }

        stage('Run New Container') {
            steps {
                sh """
                    docker run -d \
                    --name $CONTAINER_NAME \
                    -p $PORT:8080 \
                    $IMAGE_NAME:latest
                """
            }
        }

        stage('Verify Deployment') {
            steps {
                sh 'docker ps'
                sh 'docker images'
            }
        }
    }

    post {
        success {
            echo "✅ PIPELINE SUCCESS - Application Deployed"
        }
        failure {
            echo "❌ PIPELINE FAILED - Check logs"
        }
    }
}
